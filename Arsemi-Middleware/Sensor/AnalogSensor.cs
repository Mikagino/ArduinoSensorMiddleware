namespace Arsemi {
    namespace Sensor {
        public class AnalogSensor : AbstractSensor {
            public AnalogSensor(uint customID = 0) : base(customID) {
            }

            /// <summary>
            /// </summary>
            /// <returns>Nothing</returns>
            public override string ParseDataToJson() {
                return "";
            }
        }
    }
}