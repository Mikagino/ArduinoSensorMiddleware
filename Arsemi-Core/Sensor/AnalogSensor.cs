namespace Arsemi {
    namespace Sensor {
        public class AnalogSensor : AbstractSensor {
            public new const SensorTypes SensorType = SensorTypes.TYPE_GENERIC_ANALOG;
            public byte SensorPin;


            public AnalogSensor(byte sensorPin) {
                SensorPin = sensorPin;
            }
        }
    }
}