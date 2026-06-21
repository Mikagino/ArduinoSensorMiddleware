using System.IO.Ports;

namespace Arsemi {
    namespace IPC {
        public static class SerialMessaging {
            private static SerialPort? _serialPort;
            public static Action? DataReceivedAction;
            public static int ReceivedBytesThreshold {
                set {
                    if(_serialPort == null) return;
                    _serialPort.ReceivedBytesThreshold = value;
                }
                get => (_serialPort != null) ? _serialPort.ReceivedBytesThreshold : -1;
            }
            public static readonly Queue<byte> _buffer = new(64);
            private static readonly Mutex _mutex = new(false);


            /// <summary>
            /// Sets up serial communication with the microcontroller with the specified settings.
            /// </summary>
            /// <param name="serialPortInfo"></param>
            public static void Begin(SerialPortInfo serialPortInfo) {
                _serialPort = new(serialPortInfo.PortName, serialPortInfo.BaudRate) {
                    ReceivedBytesThreshold = serialPortInfo.ReceivedBytesThreshold
                };

                _serialPort.DataReceived += (_, _) => PushAllIntoBuffer();
                //_serialPort.DataReceived += (_, _) => DataReceivedAction?.Invoke();
                _serialPort.Open();
            }


            #region Reading
            /// <summary>
            /// Read the first byte from the internal serial queue
            /// </summary>
            /// <returns>return -1 when the queue is empty, otherwise the value in the queue</returns>
            public static int DequeueByte() {
                if(_buffer.TryDequeue(out byte tempResult))
                    return tempResult;
                return -1;
            }

            /// <summary>
            /// Peeks the first value of the serial queue
            /// </summary>
            /// <returns>return -1 when the queue is empty, otherwise the first value in the queue</returns>
            public static int PeekByte() {
                if(_buffer.Count == 0)
                    return -1;
                return _buffer.Peek();
            }

            /// <summary>
            /// Push all the values in the serial buffer into the internal buffer.
            /// Locks the mutex until finished. This way all values are pushed in their correct order into the buffer.
            /// </summary>
            private static void PushAllIntoBuffer() {
                _mutex.WaitOne(1000);
                try {
                    while(_serialPort?.BytesToRead > 0) {
                        _buffer.Enqueue(ReadByte());
                        Console.WriteLine(PeekByte());
                    }
                }
                finally {
                    _mutex.ReleaseMutex();
                }
                DataReceivedAction?.Invoke();
            }

            /// <summary>
            /// Reads a single byte from the serial port
            /// </summary>
            /// <returns>The byte, cast to an int</returns>
            private static byte ReadByte() {
                int result = _serialPort == null ? -1 : _serialPort.ReadByte();
                if(result == -1) throw new Exception("Byte is somehow corrupted (e.g. end of stream, value is not in valid range, ...)");
                return (byte)result;
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
                int byteCount = bytes.Length + 3;
                byte[] package = new byte[byteCount];
                Array.Copy(bytes, 0, package, 1, bytes.Length);
                package[0] = SerialProtocol.PackageStartByte;
                package[byteCount - 1] = SerialProtocol.PackageStartByte;
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
            /// Checks if a count of "byteCount" bytes are available in the buffer.
            /// </summary>
            /// <param name="byteCount"></param>
            /// <returns></returns>
            public static bool AvailableBytes(int byteCount = 1) {
                // Console.WriteLine("Still waiting for bytes...");
                return _buffer.Count >= byteCount;
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