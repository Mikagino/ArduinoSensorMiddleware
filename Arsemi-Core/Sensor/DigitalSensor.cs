namespace Arsemi {
    namespace Sensor {
        public class DigitalSensor : AbstractSensor {
            public new const SensorTypes SensorType = SensorTypes.TYPE_GENERIC_DIGITAL;
            public byte SensorPin;


            public DigitalSensor(byte sensorPin) {
                SensorPin = sensorPin;
            }


            /// <summary>
            /// Parses the sensor's data into a format for sending over serial, packaged in bytes for minimal size
            /// </summary>
            /// <returns>[SensorType, IntervalMS, SensorPin]</returns>
            public override byte[] ParseDataToByteArray() {
                return [(byte)SensorType, Data.IntervalMS, SensorPin];
            }
        }
    }
}