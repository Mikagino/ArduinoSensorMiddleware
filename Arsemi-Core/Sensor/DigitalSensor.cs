namespace Arsemi {
    namespace Sensor {
        public class DigitalSensor : AbstractSensor {
            public new const SensorTypes SensorType = SensorTypes.TYPE_GENERIC_DIGITAL;
            public byte SensorPin;


            public DigitalSensor(byte sensorPin) {
                SensorPin = sensorPin;
            }
        }
    }
}