using System.IO.MemoryMappedFiles;
using Arsemi.Sensor;

namespace Arsemi {
    namespace IPC {
        public class MemoryMappedSensorData {
            // Sensor data memory mapped file
            private const string FileDirectory = "Arsemi";
            private readonly MemoryMappedViewAccessor _file;
            private readonly string _filePath;


            /// <summary>
            /// Creates a new MemoryMappedFileAccessor for each instance
            /// </summary>
            public MemoryMappedSensorData(string fileName, long fileCapacity = 1024) {
                _filePath = Path.Combine(Path.GetTempPath(), FileDirectory);

                if(!Directory.Exists(_filePath)) {
                    Directory.CreateDirectory(_filePath);
                }
                
                _filePath = Path.Combine(_filePath, fileName);

                _file = MemoryMappedFile.CreateFromFile(_filePath, FileMode.OpenOrCreate, null, fileCapacity).CreateViewAccessor();
            }


            /// <summary>
            /// Writes sensor data into a memory mapped file.
            /// </summary>
            public void Write(SensorData data) {
                _file.Write(0, ref data);
            }


            /// <summary>
            /// </summary>
            /// <returns>Sensor data previously stored in a memory mapped file.</returns>
            public SensorData ReadAll() {
                _file.Read(0, out SensorData readData);
                return readData;
            }
        }
    }
}