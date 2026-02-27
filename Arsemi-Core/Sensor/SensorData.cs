using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public struct SensorData(uint id, List<IFilter> filters, uint interval) {
            public float Value = 0;
            public uint ID = id;
            public List<IFilter> Filters = filters;
            public uint Interval = interval;
        }
    }
}