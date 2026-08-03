using System.IO.Ports;

namespace Arsemi {
    namespace IPC {
        /// <summary>
        /// Create a new object containing all the infos of a serial port
        /// </summary>
        /// <param name="portName">How port is named on the system, keep null if you want to use the first available port (can be buggy!)</param>
        /// <param name="baudRate">How many bauds per second the serial port should sent per second</param>
        /// <param name="receivedBytesThreshold">On how many new bytes a new message counts as received</param>
        public class SerialPortInfo(string? portName = null, int baudRate = SerialProtocol.BaudRate, int receivedBytesThreshold = SerialProtocol.ReceivedBytesThreshold) {
            public string PortName { private set; get; } = portName ?? GetFirstAvailablePort();
            public int BaudRate { private set; get; } = baudRate;
            public int ReceivedBytesThreshold { private set; get; } = receivedBytesThreshold;


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            private static string GetFirstAvailablePort() {
                string[] portNames = SerialPort.GetPortNames();
                if(portNames.Length == 0) throw new Exception("No microcontroller could be found automatically. Is it connected?");
                return portNames[0];
            }
        }
    }
}