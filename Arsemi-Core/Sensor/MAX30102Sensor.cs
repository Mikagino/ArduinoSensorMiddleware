namespace Arsemi {
    namespace Sensor {
        public class MAX30102Sensor : AbstractSensor {
            public new const SensorTypes SensorType = SensorTypes.TYPE_MAX30102;


            public MAX30102Sensor() {

            }


            /// <summary>
            /// Parses the sensor's data into a format for sending over serial, packaged in bytes for minimal size
            /// </summary>
            /// <returns>[SensorType, IntervalMS]</returns>
            public override byte[] ParseDataToByteArray() {
                return [(byte)SensorType, Data.IntervalMS];
            }
        }
    }
}