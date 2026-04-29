using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public interface ISensor {
            protected void StoreValue();
            public string ParseDataToJson();
            public byte[] ParseDataToByteArray();
            public AbstractSensor AddFilter(AbstractFilter filter, string name);
            public AbstractSensor SetInterval(byte milliseconds);
            public void ApplyFilters();
        }
    }
}