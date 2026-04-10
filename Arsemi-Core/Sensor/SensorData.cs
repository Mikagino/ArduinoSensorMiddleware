using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Arsemi {
    namespace Sensor {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SensorData() {
            public float Value = 0;
            [JsonInclude] public uint ID;
            [JsonInclude] public uint IntervalMS;
        }
    }
}