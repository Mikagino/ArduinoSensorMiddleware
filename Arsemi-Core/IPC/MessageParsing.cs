using System.Threading.Tasks;

namespace Arsemi {

    namespace IPC {

        public class MessageParsing {
            private ArsemiCore _arsemiCore;

            private SerialPackage _queuedPackage = new();

            private Mutex _mutex = new(false);
            private SemaphoreSlim _semaphore = new(0, 1);
            private const int ParsingTimeoutMs = 1000;

            private DateTime _handshakeTimeout = DateTime.MinValue;
            public enum ConnectionResult {
                NONE,
                WAITING,
                SUCCESS,
                PORT_ERROR,
                TIMEOUT,
            }
            private ConnectionResult _handshakeResult = ConnectionResult.NONE;



            public MessageParsing(ArsemiCore arsemiCore) {
                _arsemiCore = arsemiCore;
            }


            /// <summary>
            /// Connects the microcontroller at the specified serial port and waits for 2 seconds to give the microcontroller time to reset.
            /// </summary>
            /// <param name="serialPortInfo"></param>
            /// <param name="drtResetWaitMs"></param>
            /// <param name="timeoutMs"></param>
            /// <returns>SUCCESS if the microcontroller is connected and the port is available, otherwise false</returns>
            public async Task<ConnectionResult> ConnectMicrocontrollerAsync(SerialPortInfo? serialPortInfo = null, int drtResetWaitMs = 2000, int timeoutMs = 5000) {
                serialPortInfo ??= new();
                Console.WriteLine("Try connecting to microcontroller on " + serialPortInfo.PortName + "...");
                SerialMessaging.Begin(serialPortInfo);
                await Task.Delay(drtResetWaitMs);
                SerialMessaging.DataReceivedAction += () => ParseMessage(); // DEBUG -> later move to start loop

                bool connected = SerialMessaging.PortAvailable();
                if(connected) { Console.WriteLine("Successfully connected to microcontroller on " + serialPortInfo.PortName); }
                else {
                    Console.WriteLine("Error connecting to microcontroller on " + serialPortInfo.PortName + "!");
                    return ConnectionResult.PORT_ERROR;
                }

                return await RequestHandshakeAsync(timeoutMs);
            }


            /// <summary>
            /// Send handshake package over serial
            /// PC sends message over port and checks if arduino replies, if yes this is the microcontroller with the Arsemi-Arduino script.
            /// </summary>
            /// <param name="waitTimeMilliseconds">For how long the method will wait for the handshake</param>
            /// <returns>SUCCESS when the connected microcontroller responded in time, WAITING if there was a handshake request already, otherwise TIMEOUT</returns>
            public async Task<ConnectionResult> RequestHandshakeAsync(int waitTimeMilliseconds) {
                if(_handshakeResult == ConnectionResult.WAITING) return ConnectionResult.WAITING;

                SerialPackage requestHandshakePackage = new SerialPackage(SerialProtocol.Action.System.RequestHandshake);
                SerialMessaging.Write(requestHandshakePackage);

                _handshakeResult = ConnectionResult.WAITING;
                _handshakeTimeout = DateTime.Now.AddMilliseconds(waitTimeMilliseconds);
                Console.WriteLine("Waiting for handshake reply...");
                return await WaitForHandshakeReplyAsync();
            }


            /// <summary>
            /// Parse new messages every 50 milliseconds until a new message is received, return if the new message is the handshake
            /// </summary>
            /// <returns>SUCCESS when _handshakeResult is set to true in the ParseMessage() due to the handshake reply, otherwise TIMEOUT</returns>
            private async Task<ConnectionResult> WaitForHandshakeReplyAsync() {
                while(_handshakeTimeout.CompareTo(DateTime.Now) > 0) {
                    if(_handshakeResult == ConnectionResult.SUCCESS) {
                        Console.WriteLine("Received handshake reply!");
                        return ConnectionResult.SUCCESS;
                    }
                }

                Console.WriteLine("Handshake timeout! Try connecting again on a different port.");
                return ConnectionResult.TIMEOUT;
            }


            /// <summary>
            /// Converts a new message from string to package and then matches the action code to the required actions
            /// </summary>
            public async Task ParseMessage() {
                if(await _semaphore.WaitAsync(ParsingTimeoutMs)) {
                    return; // failed to acquire, other thread will handle the message parsing
                }

                try {
                    while(SerialMessaging.AvailableBytes()) {

                        // Parse action code
                        if(_queuedPackage.Empty) {
                            int queuedActionCode = ParseNextActionCode();
                            if(queuedActionCode == -1)
                                break;
                            _queuedPackage.ActionCode = (byte)queuedActionCode;
                        }

                        var k = SerialMessaging.Buffer; // DEBUG
                        if(!_queuedPackage.Done)
                            await ParseNextParametersAsync();

                        bool done = false;

                        switch(_queuedPackage.ActionCode) {
                        /// Codes meant for sending to the microcontroller -> no need to implement
                        // case SerialProtocol.SystemCodes.HibernateMicrocontroller:
                        //   throw new NotImplementedException();
                        // case SerialProtocol.SystemCodes.WakeMicrocontroller:
                        //   throw new NotImplementedException();
                        // case SerialProtocol.SetupCodes.ClearConfiguration:
                        //   throw new NotImplementedException();
                        // case SerialProtocol.SetupCodes.AddSensor:
                        //   throw new NotImplementedException();
                        // case SerialProtocol.SystemCodes.RequestHandshake:
                        //   throw new NotImplementedException();

                        /// Codes meant for receiving from the microcontroller
                        case SerialProtocol.Action.System.Error:
                            done = ParseSystemError();
                            break;

                        case SerialProtocol.Action.System.ReplyHandshake:
                            if(CheckCRC8Checksum(_queuedPackage))
                                _handshakeResult = ConnectionResult.SUCCESS;
                            break;

                        case SerialProtocol.Action.Sensor.NewSample:
                            //Console.WriteLine("New sample");
                            done = ParseNewSample();
                            break;

                        case SerialProtocol.Action.Setup.SuccessfullyAddedSensor:
                            if(_queuedPackage.Parameters.Count != 1)
                                throw new Exception("Error parsing Message...");
                            int param = _queuedPackage.Parameters[0];
                            if(CheckCRC8Checksum(_queuedPackage))
                                Console.WriteLine("Successfully added a sensor on the microcontroller. -> " + param);
                            break;

                        case SerialProtocol.Action.Setup.SuccessfullyClearedConfiguration:
                            if(CheckCRC8Checksum(_queuedPackage))
                                Console.WriteLine("Successfully cleared the configuration on the microcontroller.");
                            break;

                        case SerialProtocol.Action.System.Debug:
                            if(_queuedPackage.Parameters.Count != 1)
                                break;
                            byte debugParam = _queuedPackage.Parameters[0];
                            if(CheckCRC8Checksum(_queuedPackage)) {
                                Console.Write("Debug reached! ");
                                Console.WriteLine(debugParam);
                            }
                            break;

                        case SerialProtocol.Action.System.Heartbeat:
                            if(CheckCRC8Checksum(_queuedPackage))
                                Console.WriteLine("*");
                            break;

                        default:
                            Console.WriteLine("The action code " + _queuedPackage.ActionCode + " in the message can't be associated with a command in " + nameof(SerialProtocol));
                            //throw new NotImplementedException("The action code " + _queuedActionCode + " in the message can't be associated with a command in " + nameof(SerialProtocol));
                            break;
                        }

                        if(_queuedPackage.Done)
                            _queuedPackage.Reset();
                    }
                }
                finally {
                    _queuedPackage.Reset();
                    _semaphore.Release();
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private async Task ParseNextParametersAsync() {
                for(int i = 0; ; i++) {
                    int nextByte = SerialMessaging.PeekByte();
                    if(nextByte == SerialProtocol.PackageStartByte) {
                        _queuedPackage.Crc8 = _queuedPackage.Parameters.Last();
                        _queuedPackage.Parameters.RemoveAt(_queuedPackage.Parameters.Count - 1);
                        _queuedPackage.Done = true;
                        SerialMessaging.DequeueByte(); // discard end of package
                        Console.Write("Received: [ 0 | " + _queuedPackage.ActionCode + " | ");
                        for(int j = 0; j < _queuedPackage.Parameters.Count; j++) {
                            Console.Write(_queuedPackage.Parameters[j] + " | ");
                        }
                        Console.WriteLine(_queuedPackage.Crc8 + " | 0]");
                        return;
                    }
                    else if(nextByte != -1) {
                        _queuedPackage.AppendParameter((byte)SerialMessaging.DequeueByte());
                    }
                    await Task.Delay(5);
                }
            }


            /// <summary>
            /// Parses sensorId, value and checksum from the next 3 bytes in the serial stream.
            /// </summary>
            /// <returns>false if the package doesn't have enough bytes, otherwise true (also in case of message corruption to discard the message)</returns>
            private bool ParseNewSample() {
                if(_queuedPackage.Parameters.Count != 2) {
                    throw new Exception("Error parsing package, wrong count of parameters...");
                }

                CheckCRC8Checksum(_queuedPackage);

                byte sensorId = _queuedPackage.Parameters[0];
                byte value = _queuedPackage.Parameters[1];

                Console.Write("Received sensor data from sensorId: " + sensorId + " with a value of: " + value);
                Console.WriteLine(" | Sensorname: " + _arsemiCore.Sensors[sensorId].Data.Name);
                Console.WriteLine("---");

                _arsemiCore.Sensors[sensorId].Data.Value = value;
                _arsemiCore.NewDataReceived?.Invoke(sensorId, value);
                _arsemiCore.Sensors[sensorId].CheckEventsConditions();
                return true;
            }


            /// <summary>
            /// Matches the different sensor errors to their error messages.
            /// </summary>
            /// <return></return>
            private bool ParseSystemError() {
                if(_queuedPackage.Parameters.Count < 1) {
                    throw new Exception("Error parsing package, not enough parameters...");
                }

                byte errorCode = _queuedPackage.Parameters[0];

                switch(errorCode) {
                case SerialProtocol.Error.Package.InvalidActionCode:
                    if(_queuedPackage.Parameters.Count != 3) {
                        return true; // discard package
                    }
                    CheckCRC8Checksum(_queuedPackage);
                    Console.WriteLine("Received error: " + errorCode + " = " + SerialProtocol.TryGetErrorName(errorCode) +
                                        " -> " + _queuedPackage.Parameters[1]);
                    break;
                case SerialProtocol.Error.Package.InvalidChecksum:
                    if(_queuedPackage.Parameters.Count != 3) {
                        return true; // discard package
                    }
                    CheckCRC8Checksum(_queuedPackage);
                    Console.WriteLine("Received error: " + errorCode + " = " + SerialProtocol.TryGetErrorName(errorCode)
                                        + " -> " + _queuedPackage.Parameters[1] + " != " + _queuedPackage.Parameters[2]);
                    break;
                case SerialProtocol.Error.Package.InvalidSensorParameters:
                    Console.WriteLine("Invalid sensor parameter count...");
                    break;
                default:
                    break;
                }

                return true;
            }


            /// <summary>
            /// Discards all bytes from the Serial stream until StartByte is reached.
            /// </summary>
            /// <returns>-1 when the start byte couldn't be parsed because the queue is too short</returns>
            public static int ParseNextActionCode() {
                while(SerialMessaging.AvailableBytes(2)) {
                    if(SerialMessaging.PeekByte() == SerialProtocol.PackageStartByte) {
                        _ = SerialMessaging.DequeueByte(); // discard StartByte
                        int actionCode = SerialMessaging.DequeueByte();
                        return actionCode;
                    }
                    _ = SerialMessaging.DequeueByte(); // discard
                }
                return -1;
            }

            /// <summary>
            /// Check if the next checksum byte in the serial buffer is equal to the computed checksum of the data
            /// </summary>
            /// <param name="data">data of the package, without StartByte and checksum</param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            private static bool CheckCRC8Checksum(SerialPackage package) {
                byte[] serializedPackage = package.Serialize();
                byte computedCrc8Checksum = SerialMessaging.CRC8(serializedPackage);

                if(package.Crc8 != computedCrc8Checksum) {
                    //throw new Exception("HEY! Loss of packages... :c");
                    Console.WriteLine("Loss of packages in action " + SerialProtocol.TryGetActionName(package.ActionCode) + ", checksum is not the same! -> " + package.Crc8 + "(package) != " + computedCrc8Checksum + "(computed)");
                    return false;
                }
                else return true;
            }
        }
    }
}