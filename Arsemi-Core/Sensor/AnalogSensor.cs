namespace Arsemi {
    namespace Sensor {
        public class AnalogSensor : AbstractSensor {
            public new const SensorTypes SensorType = SensorTypes.TYPE_GENERIC_ANALOG;
            public byte SensorPin;


            public AnalogSensor(byte sensorPin) {
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