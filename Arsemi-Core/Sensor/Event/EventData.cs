namespace Arsemi {
    namespace Sensor {
        namespace Event {
            public struct EventData(string name, AbstractSensor? sensor = null) {
                public string Name = name;
                public AbstractSensor? Sensor = sensor;
            }
        }
    }
}