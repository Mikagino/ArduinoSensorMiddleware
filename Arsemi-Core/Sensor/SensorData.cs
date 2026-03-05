using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SensorData() {
            [JsonInclude] public float Value = 0;
            [JsonInclude] public uint ID;
            [JsonInclude] public uint Interval;
            //public fixed FilterData Filters[8];
        }
    }
}