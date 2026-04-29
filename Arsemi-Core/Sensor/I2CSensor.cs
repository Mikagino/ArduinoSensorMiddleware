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
        }
    }
}