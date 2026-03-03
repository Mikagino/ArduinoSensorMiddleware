using System.Runtime.InteropServices;
using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct SensorData() {
            public float Value = 0;
            public uint ID;
            public uint Interval;
            //public fixed FilterData Filters[8];
        }
    }
}