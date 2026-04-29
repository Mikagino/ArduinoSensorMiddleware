namespace Arsemi {
    namespace Sensor {
        public class I2CSensor : AbstractSensor {
            public new const SensorTypes SensorType = SensorTypes.TYPE_GENERIC_I2C;


            private byte _adress;
            private byte _reg;
            private byte _bytes;


            public I2CSensor(byte address = 0x00, byte reg = 0x00, byte bytes = 2) {
                _adress = address;
                _reg = reg;
                _bytes = bytes;
            }


            /// <summary>
            /// Parses the sensor's data into a format for sending over serial, packaged in bytes for minimal size
            /// </summary>
            /// <returns>[SensorType, IntervalMS, Adress, Reg, Bytes]</returns>
            public override byte[] ParseDataToByteArray() {
                return [(byte)SensorType, Data.IntervalMS, _adress, _reg, _bytes];
            }
        }
    }
}