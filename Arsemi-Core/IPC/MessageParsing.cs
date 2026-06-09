using System.ComponentModel;
using System.IO.Ports;
using BenchmarkDotNet.Attributes;

namespace Arsemi {

    namespace IPC {

        public class MessageParsing {
            private ArsemiCore _arsemiCore;
            private int _queuedActionCode = -1;

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
                Console.WriteLine("ArSeMi: Try connecting to microcontroller on " + serialPortInfo.PortName);
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

                // foreach(byte b in _serialMessaging.ReadBytes()) {
                //   Console.Write(b.ToString());
                //   Console.Write("-");
                // }
                // Console.Write("---");
                while(SerialMessaging.AvailableBytes()) {

                    if(_queuedActionCode == -1) {
                        _queuedActionCode = ParsePackageStart();
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
                        if(CheckNextCRC8Checksum((byte)_queuedActionCode))
                            Console.WriteLine("Successfully added a sensor on the microcontroller.");
                        done = true;
                        break;

                    case SerialProtocol.Action.Setup.SuccessfullyClearedConfiguration:
                        if(CheckNextCRC8Checksum((byte)_queuedActionCode))
                            Console.WriteLine("Successfully cleared the configuration on the microcontroller.");
                        done = true;
                        break;

                    case SerialProtocol.Action.System.Debug:
                        byte debugParam = SerialMessaging.ReadByte();
                        if(CheckNextCRC8Checksum((byte)_queuedActionCode, debugParam)) {
                            Console.Write("Debug reached! ");
                            Console.WriteLine(debugParam);
                        }
                        done = true;
                        break;

                    default:
                        throw new NotImplementedException("The action code in the message can't be associated with a command in " + nameof(SerialProtocol));
                    }

                    if(done) _queuedActionCode = -1;
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

                byte sensorId = SerialMessaging.ReadByte();
                byte value = SerialMessaging.ReadByte();

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

                byte errorCode = SerialMessaging.ReadByte();

                switch(errorCode) {
                case SerialProtocol.Error.Package.InvalidActionCode:
                    byte invalidCode = SerialMessaging.ReadByte();
                    CheckNextCRC8Checksum(SerialProtocol.Action.System.Error, errorCode, invalidCode);
                    Console.WriteLine("Received error: " + errorCode + " = " + SerialProtocol.TryGetErrorName(errorCode) + " -> " + invalidCode);
                    break;
                case SerialProtocol.Error.Package.InvalidChecksum:
                    byte invalidCurrentChecksum = SerialMessaging.ReadByte();
                    byte invalidComputedChecksum = SerialMessaging.ReadByte();
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
            /// Reads from Serial until value is reached 
            /// </summary>
            /// <param name="value"></param>
            /// <returns>true when value is found in the stream, otherwise false</returns>
            private static bool DiscardUntilValue(byte value) {
                while(SerialMessaging.AvailableBytes()) {
                    byte message = SerialMessaging.ReadByte();
                    if(message == value) {
                        return true;
                    }
                }
                return false;
            }


            /// <summary>
            /// Discards all bytes from the Serial stream until @packageStartByte is reached.
            /// </summary>
            /// <param name="packageStartByte"></param>
            /// <returns>-1 when the package is not yet finished</returns>
            public static int ParsePackageStart(byte packageStartByte = SerialProtocol.PackageStartByte) {
                if(DiscardUntilValue(packageStartByte)) {
                    return SerialMessaging.ReadByte();
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
                byte currentCrc8Checksum = SerialMessaging.ReadByte();
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