using System.IO.Ports;
using System.Numerics;

namespace Arsemi {
    namespace IPC {
        public class SerialMessaging {
            private SerialPort? _serialPort;
            public SerialDataReceivedEventHandler? DataReceived;
            public Action? DataReceivedAction;
            private Action? _waitingAction;


            public int ReceivedBytesThreshold {
                set => _serialPort.ReceivedBytesThreshold = value;
                get => _serialPort.ReceivedBytesThreshold;
            }


            /// <summary>
            /// Sets up serial communication with the microcontroller with the specified settings.
            /// </summary>
            /// <param name="portName">Must match exactly with the string found in SerialPort.GetPortNames().</param>
            /// <param name="baudRate">Only change if you changed it also on the Microcontroller!</param>
            /// <param name="receivedBytesThreshold">Action is invoked when the message byte size is higher than this threshold.</param>
            public void Begin(string portName, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold) {
                _serialPort = new(portName, baudRate) {
                    ReceivedBytesThreshold = receivedBytesThreshold
                };

                _serialPort.DataReceived += ReadLine; // DEBUG?
                _serialPort.Open();
            }


            /// <summary>
            /// TODO: Reads a line from the USB-Port for reading data send from arduino
            /// </summary>
            private void ReadLine(object _, SerialDataReceivedEventArgs e) {
                DataReceived?.Invoke(_, e);
                DataReceivedAction?.Invoke();
                // CheckPort();
                // string arduinoMessage = _serialPort.ReadLine();
                // Console.WriteLine("Arduino says: " + arduinoMessage);
            }

            public string ReadLine() {
                CheckPort();
                if(_serialPort.BytesToRead < 0) {
                    return "";
                }

                return _serialPort.ReadLine();
            }


            public byte ReadByte() {
                CheckPort();
                return (byte)_serialPort.ReadByte();
            }


            /// <summary>
            /// Reads all bytes available in the stream
            /// </summary>
            /// <param name="oneLine"></param>
            /// <returns></returns>
            public byte[] ReadBytes() {
                CheckPort();
                if(!AvailableBytes()) {
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
            /// Write a line over the serial port
            /// </summary>
            /// <param name="text"></param>
            public void WriteLine(string text) {
                CheckPort();
                _serialPort.WriteLine(text);
            }


            /// <summary>
            /// Writes bytes over the serial port (way faster than WriteLine if data is serialized)
            /// </summary>
            /// <param name="bytes"></param>
            public void WriteBytes(byte[] bytes) {
                CheckPort();
                _serialPort.Write(bytes, 0, bytes.Length);
            }


            /// <summary>
            /// TODO: add more checks |
            /// Checks if the serial port is actually created
            /// </summary>
            /// <exception cref="Exception"></exception>
            private void CheckPort() {
                if(_serialPort == null || !_serialPort.IsOpen) {
                    throw new Exception("Serial port is not open yet! ;^; Call Begin() before reading...");
                }
            }


            /// <summary>
            /// TODO: Make it work... xD
            /// PC sends message over port and checks if arduino replies, if yes this is the microcontroller with the Arsemi-Arduino script.
            /// </summary>
            /// <param name="finishAction"></param>
            private void RequestHandshake(Action? finishAction) {
                // string requestHandshakeMessage = SerialProtocol.CombineToMessage(0, SerialProtocol.SetupCodes.AddSensor);
                // WriteLine(requestHandshakeMessage);
                _waitingAction += finishAction;
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