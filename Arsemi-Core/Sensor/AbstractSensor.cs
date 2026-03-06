using System.Text.Json.Serialization;
using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            [JsonInclude] public SensorData Data = new();
            [JsonInclude] public Dictionary<string, AbstractFilter> Filters = [];

            protected static List<uint> _previouslyGeneratedIDs = [];

            protected List<double> _samples = [];

            public enum EventType {
                Threshold,
            }


            public AbstractSensor(uint sampleRange = 256, uint customID = 0) {
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
            public AbstractSensor AddFilter(AbstractFilter filter, string name) {
                Filters ??= [];
                Filters.Add(name, filter);
                return this;
            }


            public AbstractSensor SetInterval(uint milliseconds) {
                Data.IntervalMS = milliseconds;
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