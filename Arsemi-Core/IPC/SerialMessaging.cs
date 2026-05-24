using System.IO.Ports;
using System.Threading.Tasks;

namespace Arsemi {
    namespace IPC {
        public class SerialMessaging {
            private SerialPort? _serialPort;
            public Action? DataReceivedAction;
            private Action? _waitingAction;
            private SerialPackage[] _packageBuffer = [];


            public int ReceivedBytesThreshold {
                set {
                    if(!PortAvailable()) return;
                    _serialPort.ReceivedBytesThreshold = value;
                }
                get {
                    if(!PortAvailable()) return -1;
                    return _serialPort.ReceivedBytesThreshold;
                }
            }


            /// <summary>
            /// Sets up serial communication with the microcontroller with the specified settings.
            /// </summary>
            /// <param name="portName">Must match exactly with the string found in SerialPort.GetPortNames().</param>
            /// <param name="baudRate">Only change if you changed it also on the Microcontroller!</param>
            /// <param name="receivedBytesThreshold">Action is invoked when the message byte size is higher than this threshold.</param>
            public async Task Begin(string portName, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold) {
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
            /// <returns>byte which has been read</returns>
            public byte ReadByte() {
                return (byte)_serialPort.ReadByte();
            }


            /// <summary>
            /// TODO: Reads all bytes available in the stream
            /// </summary>
            /// <returns></returns>
            public byte[] ReadBytes() {
                if(!PortAvailable() || !AvailableBytes()) {
                    return [];
                }

                // int nextByte;

                byte[] bytes = new byte[_serialPort.BytesToRead];
                for(int i = 0; i < _serialPort.BytesToRead; i++) {
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


            /// <summary>
            /// Reads from Serial until value is reached 
            /// </summary>
            /// <param name="value"></param>
            /// <returns>true when value is found in the stream, otherwise false</returns>
            private bool DiscardUntilValue(byte value) {
                while(AvailableBytes()) {
                    byte message = ReadByte();
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
            public int ParsePackageStart(byte packageStartByte = SerialProtocol.PackageStartByte) {
                if(DiscardUntilValue(packageStartByte)) {
                    return ReadByte();
                }
                return -1;
            }

            #endregion Reading


            #region Writing
            /// <summary>
            /// Writes an array of bytes over the serial port, serialized after one another.
            /// Serialization-Protocol: [SerialProtocol.StartByte, bytes, CRC8]
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns>Every byte of the sent message, including (StartByte + Message + CRC8)</returns>
            public byte[] Write(params byte[] bytes) {
                if(!PortAvailable()) return [];
                byte[] package = new byte[bytes.Length + 2];
                Array.Copy(bytes, 0, package, 1, bytes.Length);
                package[0] = SerialProtocol.PackageStartByte;
                package[bytes.Length + 1] = CRC8(bytes);
                _serialPort.Write(package, 0, package.Length);
                Console.Write("Written: ");
                foreach(byte b in package) {
                    Console.Write(b + "-");
                }
                Console.Write("\n");
                return package;
            }


            /// <summary>
            /// Writes an array of bytes over the serial port, serialized after one another.
            /// Serialization-Protocol: [SerialProtocol.StartByte, package, CRC8]
            /// </summary>
            /// <param name="package">the package to be sent</param>
            /// <returns>serialized package bytes</returns>
            public byte[] Write(SerialPackage package) {
                return Write(package.Serialize());
            }

            #endregion Writing



            /// <summary>
            /// TODO: add more checks |
            /// Checks if the serial port is actually created and open
            /// </summary>
            public bool PortAvailable() {
                return _serialPort != null && _serialPort.IsOpen;
            }


            /// <summary>
            /// Checks if a count of "byteCount" bytes are available in the buffer where.
            /// </summary>
            /// <param name="byteCount"></param>
            /// <returns></returns>
            public bool AvailableBytes(int byteCount = 1) {
                // Console.WriteLine("Still waiting for bytes...");
                return _serialPort.BytesToRead >= byteCount;
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