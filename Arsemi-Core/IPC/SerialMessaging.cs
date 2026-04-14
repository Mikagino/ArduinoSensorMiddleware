using System.IO.Ports;

namespace Arsemi {
    namespace IPC {
        public class SerialMessaging {
            private SerialPort? _serialPort;
            public SerialDataReceivedEventHandler? DataReceived;
            private Action? _waitingAction;


            /// <summary>
            /// Sets up serial communication with the microcontroller with the specified settings.
            /// </summary>
            /// <param name="portName">Must match exactly with the string found in SerialPort.GetPortNames().</param>
            /// <param name="baudRate">Only change if you changed it also on the Microcontroller!</param>
            /// <param name="receivedBytesThreshold">Action is invoked when the message byte size is higher than this threshold.</param>
            public void Begin(string portName, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = 5) {
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
                // CheckPort();
                // string arduinoMessage = _serialPort.ReadLine();
                // Console.WriteLine("Arduino says: " + arduinoMessage);
            }

            public string ReadLine() {
                CheckPort();
                return _serialPort.ReadLine();
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
            /// TODO: add more checks |
            /// Checks if the serial port is actually created
            /// </summary>
            /// <exception cref="Exception"></exception>
            private void CheckPort() {
                if(_serialPort == null) {
                    throw new Exception("Serial port is not open yet! ;^; Call Begin() before reading...");
                }
            }


            /// <summary>
            /// TODO: Make it work... xD
            /// PC sends message over port and checks if arduino replies, if yes this is the microcontroller with the Arsemi-Arduino script.
            /// </summary>
            /// <param name="finishAction"></param>
            private void RequestHandshake(Action? finishAction) {
                string requestHandshakeMessage = SerialProtocol.CombineToMessage(0, SerialProtocol.SetupCodes.AddSensor);
                WriteLine(requestHandshakeMessage);
                _waitingAction += finishAction;
            }
        }
    }
}