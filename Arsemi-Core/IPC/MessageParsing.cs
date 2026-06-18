namespace Arsemi {

    namespace IPC {

        public class MessageParsing {
            private ArsemiCore _arsemiCore;
            private int _queuedActionCode = -1;

            private Mutex _mutex = new(false);
            private const int MutexTimeoutMs = 1000;

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
                SerialMessaging.DataReceivedAction += ParseMessage; // DEBUG -> later move to start loop

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
            public void ParseMessage() {
                _mutex.WaitOne(MutexTimeoutMs);
                // foreach(byte b in _serialMessaging.ReadBytes()) {
                //   Console.Write(b.ToString());
                //   Console.Write("-");
                // }
                // Console.Write("---");
                try {

                    while(SerialMessaging.AvailableBytes()) {

                        if(_queuedActionCode == -1) {
                            _queuedActionCode = ParseNextActionCode();
                            //if(_queuedActionCode != -1) Console.WriteLine("Received new action = " + _queuedActionCode);

                        }

                        if(_queuedActionCode == -1) {
                            return;
                        }

                        bool done = false;

                        switch(_queuedActionCode) {
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
                            if(CheckNextCRC8Checksum((byte)_queuedActionCode))
                                _handshakeResult = ConnectionResult.SUCCESS;
                            done = true;
                            break;

                        case SerialProtocol.Action.Sensor.NewSample:
                            //Console.WriteLine("New sample");
                            done = ParseNewSample();
                            break;

                        case SerialProtocol.Action.Setup.SuccessfullyAddedSensor:
                            if(!SerialMessaging.AvailableBytes(2))
                                break;
                            int param = SerialMessaging.DequeueByte();
                            if(CheckNextCRC8Checksum((byte)_queuedActionCode, (byte)param))
                                Console.WriteLine("Successfully added a sensor on the microcontroller. -> " + param);
                            done = true;
                            break;

                        case SerialProtocol.Action.Setup.SuccessfullyClearedConfiguration:
                            if(!SerialMessaging.AvailableBytes(1))
                                break;
                            if(CheckNextCRC8Checksum((byte)_queuedActionCode))
                                Console.WriteLine("Successfully cleared the configuration on the microcontroller.");
                            done = true;
                            break;

                        case SerialProtocol.Action.System.Debug:
                            if(!SerialMessaging.AvailableBytes(2))
                                break;
                            byte debugParam = (byte)SerialMessaging.DequeueByte();
                            if(CheckNextCRC8Checksum((byte)_queuedActionCode, debugParam)) {
                                Console.Write("Debug reached! ");
                                Console.WriteLine(debugParam);
                            }
                            done = true;
                            break;

                        case SerialProtocol.Action.System.Heartbeat:
                            if(CheckNextCRC8Checksum((byte)_queuedActionCode))
                                Console.WriteLine("*");
                            done = true;
                            break;

                        default:
                            Console.WriteLine("The action code " + _queuedActionCode + " in the message can't be associated with a command in " + nameof(SerialProtocol));
                            //throw new NotImplementedException("The action code " + _queuedActionCode + " in the message can't be associated with a command in " + nameof(SerialProtocol));
                            done = true;
                            break;
                        }

                        if(done) _queuedActionCode = -1;
                    }
                }
                finally {
                    _mutex.ReleaseMutex();
                }
            }


            /// <summary>
            /// Parses sensorId, value and checksum from the next 3 bytes in the serial stream.
            /// </summary>
            /// <returns>false if the package doesn't have enough bytes, otherwise true (also in case of message corruption to discard the message)</returns>
            private bool ParseNewSample() {
                if(!SerialMessaging.AvailableBytes(3)) {
                    return false;
                }

                byte sensorId = (byte)SerialMessaging.DequeueByte();
                byte value = (byte)SerialMessaging.DequeueByte();

                CheckNextCRC8Checksum(SerialProtocol.Action.Sensor.NewSample, sensorId, value);

                Console.Write("Received sensor data from sensorId: " + sensorId + " with a value of: " + value);
                Console.WriteLine(" | Sensorname: " + _arsemiCore.Sensors[sensorId].Data.Name);
                Console.WriteLine("---");

                _arsemiCore.Sensors[sensorId].Data.Value = value;
                _arsemiCore.NewDataReceived?.Invoke(sensorId, value);
                return true;
            }


            /// <summary>
            /// Matches the different sensor errors to their error messages.
            /// </summary>
            /// <return></return>
            private bool ParseSystemError() {
                if(!SerialMessaging.AvailableBytes(1)) {
                    return false;
                }

                byte errorCode = (byte)SerialMessaging.DequeueByte();

                switch(errorCode) {
                case SerialProtocol.Error.Package.InvalidActionCode:
                    byte invalidCode = (byte)SerialMessaging.DequeueByte();
                    CheckNextCRC8Checksum(SerialProtocol.Action.System.Error, errorCode, invalidCode);
                    Console.WriteLine("Received error: " + errorCode + " = " + SerialProtocol.TryGetErrorName(errorCode) + " -> " + invalidCode);
                    break;
                case SerialProtocol.Error.Package.InvalidChecksum:
                    byte invalidCurrentChecksum = (byte)SerialMessaging.DequeueByte();
                    byte invalidComputedChecksum = (byte)SerialMessaging.DequeueByte();
                    CheckNextCRC8Checksum(SerialProtocol.Action.System.Error, errorCode, invalidCurrentChecksum, invalidComputedChecksum);
                    Console.WriteLine("Received error: " + errorCode + " = " + SerialProtocol.TryGetErrorName(errorCode)
                                        + " -> " + invalidCurrentChecksum + " != " + invalidComputedChecksum
                                        );
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
            private static bool CheckNextCRC8Checksum(params byte[] data) {
                int currentCrc8Checksum = SerialMessaging.DequeueByte();
                byte computedCrc8Checksum = SerialMessaging.CRC8(data);

                if(currentCrc8Checksum != computedCrc8Checksum) {
                    //throw new Exception("HEY! Loss of packages... :c");
                    Console.WriteLine("Loss of packages in action " + SerialProtocol.TryGetActionName(data[0]) + ", checksum is not the same! -> " + currentCrc8Checksum + " != " + computedCrc8Checksum);
                    return false;
                }
                else return true;
            }
        }
    }
}