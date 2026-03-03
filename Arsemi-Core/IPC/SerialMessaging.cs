using System.IO.Ports;

namespace Arsemi {
    namespace IPC {
        public class SerialMessaging {
            private readonly SerialPort _serialPort;


            public SerialMessaging(string portName, int baudRate, int receivedBytesThreshold = 5) {
                _serialPort = new(portName, baudRate) {
                    ReceivedBytesThreshold = receivedBytesThreshold
                };
                _serialPort.DataReceived += ReadLineSerial;
                _serialPort.Open();
            }


            /// <summary>
            /// TODO: Reads a line from the USB-Port for reading data send from arduino
            /// </summary>
            private void ReadLineSerial(object _, SerialDataReceivedEventArgs e) {
                string arduinoMessage = _serialPort.ReadLine();
                Console.WriteLine(arduinoMessage);
            }
        }
    }
}