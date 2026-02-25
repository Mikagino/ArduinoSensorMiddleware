using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            public uint ID { private set; get; }
            private List<IFilter> _filters = [];
            public uint Interval = 100;
            protected static List<uint> _previouslyGeneratedIDs = [];


            public AbstractSensor(uint customID = 0) {
                if(customID != 0) {
                    ID = GenerateID();
                }

                _previouslyGeneratedIDs.Add(ID);
            }


            /// <summary>
            /// Stores the value read from serial into the shared memory
            /// </summary>
            public void StoreValue() {

            }

            /// <summary>
            /// Implement parsing to JSON
            /// </summary>
            /// <returns></returns>
            public virtual string ParseDataToJson() {
                return "";
            }


            /// <summary>
            /// Adds a specific filter to the filterstack (all of them will be executed)
            /// </summary>
            /// <returns></returns>
            public AbstractSensor AddFilter(IFilter filter) {
                _filters.Add(filter);
                return this;
            }


            public AbstractSensor SetInterval(uint milliseconds) {
                Interval = milliseconds;
                return this;
            }


            /// <summary>
            /// </summary>
            /// <returns>Unique number for each new instance of the class (currently simply counting up)</returns>
            private uint GenerateID() {
                return (uint)_previouslyGeneratedIDs.Count;
            }
        }
    }
}