using System.IO.Ports;

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
            /// <param name="portName">If null the first accessible port is used.</param>
            /// <param name="baudRate"></param>
            /// <param name="receivedBytesThreshold"></param>
            /// <returns>true if the microcontroller is connected and the port is available, otherwise false</returns>
            /// <exception cref="Exception"></exception>
            public async Task<ConnectionResult> ConnectMicrocontrollerAsync(string? portName = null, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold, int drtResetWaitMs = 2000, int timeoutMs = 5000) {
                if(portName == null) {
                    string[] portNames = SerialPort.GetPortNames();
                    if(portNames.Length == 0) throw new Exception("No microcontroller could be found automatically. Is it connected?");
                    portName = portNames[0];
                }
                Console.WriteLine("ArSeMi: Try connecting to microcontroller on " + portName);
                await SerialMessaging.Begin(portName, baudRate, receivedBytesThreshold);
                SerialMessaging.DataReceivedAction += ParseMessage; // DEBUG -> later move to start loop

                bool connected = SerialMessaging.PortAvailable();
                if(connected) { Console.WriteLine("Successfully connected to microcontroller on " + portName); }
                else {
                    Console.WriteLine("Error connecting to microcontroller on " + portName + "!");
                    return MessageParsing.ConnectionResult.PORT_ERROR;
                }

                return await RequestHandshakeAsync(timeoutMs);
            }


            /// <summary>
            /// Send handshake package over serial
            /// PC sends message over port and checks if arduino replies, if yes this is the microcontroller with the Arsemi-Arduino script.
            /// </summary>
            /// <param name="waitTimeMilliseconds">For how long the method will wait for the handshake</param>
            /// <returns>SUCCESS when the connected microcontroller responded in time, WAITING if there was a handshake request already, otherwise TIMEOUT</returns>
            private async Task<ConnectionResult> RequestHandshakeAsync(int waitTimeMilliseconds) {
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
                    }

                    if(_queuedActionCode == -1) {
                        return;
                    }

                    else Console.WriteLine("New action! = " + _queuedActionCode);


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
                    case SerialProtocol.Action.System.SystemError:
                        ParseSystemError();
                        break;

                    case SerialProtocol.Action.System.ReplyHandshake:
                        _handshakeResult = ConnectionResult.SUCCESS;
                        break;

                    case SerialProtocol.Action.Sensor.NewSample:
                        Console.WriteLine("New sample");
                        ParseNewSample();
                        break;

                    // case SerialProtocol.SetupCodes.SuccessfullyAddedSensor:

                    default:
                        Console.WriteLine("Queued action code: " + _queuedActionCode);
                        _queuedActionCode = -1;
                        break;
                        // default:
                        //   throw new NotImplementedException("The action code in the message can't be associated with a command.");
                    }
                }
            }


            /// <summary>
            /// Parses sensorId, value and checksum from the next 3 bytes in the serial stream.
            /// </summary>
            private void ParseNewSample() {
                if(!SerialMessaging.AvailableBytes(3)) {
                    return;
                }

                byte sensorId = SerialMessaging.ReadByte();
                byte value = SerialMessaging.ReadByte();
                byte checksum = SerialMessaging.ReadByte();

                byte computedChecksum = SerialMessaging.CRC8(SerialProtocol.Action.Sensor.NewSample, sensorId, value);

                if(checksum != computedChecksum) {
                    throw new Exception("HEY! Loss of packages... :c");
                }

                Console.Write("Received sensor data from sensorId: " + sensorId + " with a value of: " + value);
                Console.WriteLine(" | Sensorname: " + _arsemiCore.Sensors[sensorId].Data.Name);
                Console.WriteLine("---");

                _arsemiCore.Sensors[sensorId].Data.Value = value;
                _arsemiCore.NewDataReceived?.Invoke(sensorId, value);
                _queuedActionCode = -1;
            }


            /// <summary>
            /// Matches the different sensor errors to their error messages.
            /// </summary>
            private void ParseSystemError() {
                if(!SerialMessaging.AvailableBytes(1)) {
                    return;
                }

                byte errorCode = SerialMessaging.ReadByte();

                switch(errorCode) {
                default:
                    Console.WriteLine("Received sensor error code: " + errorCode + " = " + SerialProtocol.TryGetActionName(errorCode));
                    break;
                }

                _queuedActionCode = -1;
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
        }
    }
}