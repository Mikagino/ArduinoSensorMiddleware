using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            public SensorData Data = new();
            protected static List<uint> _previouslyGeneratedIDs = [];

            public enum EventType {
                Threshold,
            }


            public AbstractSensor(uint customID = 0) {
                if(customID != 0) {
                    Data.ID = GenerateID();
                }

                _previouslyGeneratedIDs.Add(Data.ID);
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
                Data.Filters.Add(filter);
                return this;
            }


            public AbstractSensor SetInterval(uint milliseconds) {
                Data.Interval = milliseconds;
                return this;
            }


            /// <summary>
            /// </summary>
            /// <returns>Unique number for each new instance of the class (currently simply counting up)</returns>
            private uint GenerateID() {
                return (uint)_previouslyGeneratedIDs.Count;
            }

            /// <summary>
            /// TODO: Adds a new event (base function -> needs implementations for specific event types, maybe use class instead of enum?)
            /// </summary>
            public void AddEvent(EventType eventType, string name, int value) {
                throw new NotImplementedException();
            }
        }
    }
}