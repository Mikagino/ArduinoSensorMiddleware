using System.IO.Ports;

namespace Arsemi {
    namespace IPC {
        public static class SerialMessaging {
            private static SerialPort? _serialPort;
            public static Action? DataReceivedAction;
            public static int ReceivedBytesThreshold {
                set => _serialPort?.ReceivedBytesThreshold = value;
                get => (_serialPort != null) ? _serialPort.ReceivedBytesThreshold : -1;
            }


            /// <summary>
            /// Sets up serial communication with the microcontroller with the specified settings.
            /// </summary>
            /// <param name="portName">Must match exactly with the string found in SerialPort.GetPortNames().</param>
            /// <param name="baudRate">Only change if you changed it also on the Microcontroller!</param>
            /// <param name="receivedBytesThreshold">Action is invoked when the message byte size is higher than this threshold.</param>
            public static async Task Begin(string portName, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold) {
                _serialPort = new(portName, baudRate) {
                    ReceivedBytesThreshold = receivedBytesThreshold
                };

                _serialPort.DataReceived += (_, _) => DataReceivedAction?.Invoke();
                _serialPort.Open();
                await Task.Delay(2000);
            }


            #region Reading
            /// <summary>
            /// Reads a single byte from the serial port
            /// </summary>
            /// <returns>The byte, cast to an int, or -1 if the end of the stream has been read or _serialPort is null.</returns>
            public static byte ReadByte() {
                int result = _serialPort == null ? -1 : _serialPort.ReadByte();
                if(result == -1) throw new Exception("Byte is somehow corrupted (e.g. end of stream, value is not in valid range, ...)");
                return (byte)result;
            }


            /// <summary>
            /// TODO: Reads all bytes available in the stream
            /// </summary>
            /// <returns></returns>
            public static int[] ReadBytes() {
                if(!PortAvailable() || !AvailableBytes()) {
                    return [];
                }

                int bytesToRead = _serialPort == null ? 0 : _serialPort.BytesToRead;
                if(bytesToRead == 0) return [];

                int[] bytes = new int[bytesToRead];
                for(int i = 0; i < bytesToRead; i++) {
                    bytes[i] = ReadByte();
                }
                // char endline = _serialPort.NewLine.ToCharArray()[0];

                // // for(int i = 0; i < _serialPort.BytesToRead; i++) {
                // //     nextByte = _serialPort.ReadByte();
                // //     // string? a = _serialPort.ReadLine();
                // //     // Console.WriteLine(a);
                // //     if(nextByte == -1 || nextByte == endline) {
                // //         break;
                // //     }
                // //     bytes.Add((byte)nextByte);
                // //     Console.WriteLine((byte)nextByte);
                // // }
                // _serialPort.Read(bytes, 0, _serialPort.BytesToRead);
                // foreach(byte b in bytes) {
                //     Console.Write(b.ToString() + ", ");
                // }
                // Console.WriteLine("\n---");

                return bytes;
            }
            #endregion Reading


            #region Writing
            /// <summary>
            /// Writes an array of bytes over the serial port, serialized after one another.
            /// Serialization-Protocol: [SerialProtocol.StartByte, bytes, CRC8]
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns>Every byte of the sent message, including (StartByte + Message + CRC8)</returns>
            public static byte[] Write(params byte[] bytes) {
                if(!PortAvailable()) return [];
                byte[] package = new byte[bytes.Length + 2];
                Array.Copy(bytes, 0, package, 1, bytes.Length);
                package[0] = SerialProtocol.PackageStartByte;
                package[bytes.Length + 1] = CRC8(bytes);
                _serialPort?.Write(package, 0, package.Length);

                // Debug print
                Console.Write("Sent: [");
                string? actionName;
                for(int i = 0; i < package.Length; i++) {
                    Console.Write(package[i]);
                    if(i < package.Length - 1) Console.Write(" | ");
                }
                actionName = SerialProtocol.TryGetActionName(package[1]);
                Console.Write("] = " + (actionName ?? "ERROR"));
                Console.Write("\n");

                return package;
            }


            /// <summary>
            /// Writes an array of bytes over the serial port, serialized after one another.
            /// Serialization-Protocol: [SerialProtocol.StartByte, package, CRC8]
            /// </summary>
            /// <param name="package">the package to be sent</param>
            /// <returns>serialized package bytes</returns>
            public static byte[] Write(SerialPackage package) {
                return Write(package.Serialize());
            }

            #endregion Writing


            /// <summary>
            /// TODO: add more checks |
            /// Checks if the serial port is actually created and open
            /// </summary>
            /// <returns></returns>
            public static bool PortAvailable() {
                return _serialPort != null && _serialPort.IsOpen;
            }


            /// <summary>
            /// Checks if a count of "byteCount" bytes are available in the buffer where.
            /// </summary>
            /// <param name="byteCount"></param>
            /// <returns></returns>
            public static bool AvailableBytes(int byteCount = 1) {
                // Console.WriteLine("Still waiting for bytes...");
                return _serialPort?.BytesToRead >= byteCount;
            }


            /// <summary>
            /// CRC-8 checksum generator based on the code by devcoons (Source: https://devcoons.com/crc8/)
            /// </summary>
            /// <param name="data"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static byte CRC8(params byte[] data) {
                byte crc = 0x00;
                for(int i = 0; i < data.Length; i++) {
                    byte extract = data[i];
                    for(int tempI = 8; tempI > 0; tempI--) {
                        byte sum = (byte)((crc ^ extract) & 0x01);
                        crc = (byte)(crc >> 1);
                        if(sum != 0) {
                            crc ^= 0x8C;
                        }
                        extract = (byte)(extract >> 1);
                    }
                }
                return crc;
            }

        }
    }
}