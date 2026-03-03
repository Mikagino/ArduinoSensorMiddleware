using System.Runtime.Serialization.Formatters;
using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public interface ISensor {
            protected void StoreValue();
            public string ParseDataToJson();
            public AbstractSensor AddFilter(AbstractFilter filter);
            public AbstractSensor SetInterval(uint milliseconds);
        }
    }
}