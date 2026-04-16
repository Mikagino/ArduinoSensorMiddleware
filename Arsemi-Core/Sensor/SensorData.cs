using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Arsemi {
    namespace Sensor {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SensorData() {
            public byte Value = 0;
            [JsonInclude] public byte ID;
            [JsonInclude] public byte IntervalMS;
        }
    }
}