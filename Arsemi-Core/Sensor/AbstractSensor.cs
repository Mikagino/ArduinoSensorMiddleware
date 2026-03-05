using System.Text.Json.Serialization;
using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            [JsonInclude] public SensorData Data = new();
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
            /// </summary>
            /// <returns>TODO: Complete sensor data as JSON string.</returns>
            public string ParseDataToJson() {
                return System.Text.Json.JsonSerializer.Serialize(this);
            }


            /// <summary>
            /// Adds a specific filter to the filterstack (all of them will be executed)
            /// </summary>
            /// <returns></returns>
            public AbstractSensor AddFilter(AbstractFilter filter) {
                // Data.Filters ??= [];
                // for(int i = 0; i < Data.Filters.Length; i++) {
                //     if(!Data.Filters[i].Added) {
                //         Data.Filters[i] = filter.Data;
                //         break;
                //     }
                //     if(i == Data.Filters.Length) {
                //         throw new OverflowException("Too many filters added!");
                //     }
                // }
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
                return (uint)_previouslyGeneratedIDs.Count + 1;
            }

            /// <summary>
            /// TODO: Adds a new event (base function -> needs implementations for specific event types, maybe use class instead of enum?)
            /// </summary>
            public void AddEvent(EventType eventType, string name, int value) {
            }
        }
    }
}