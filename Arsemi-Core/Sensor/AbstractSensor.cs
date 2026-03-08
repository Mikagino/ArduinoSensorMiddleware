using System.Text.Json.Serialization;
using Arsemi.Sensor.Filter;
using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            [JsonInclude] public SensorData Data = new();
            [JsonInclude] public Dictionary<string, AbstractFilter> Filters = [];


            protected static List<uint> _previouslyGeneratedIDs = [];


            #region Samples
            public Utilities.RingBuffer RawBuffer = new();
            public Utilities.RingBuffer FilteredBuffer = new();
            #endregion Samples


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


            #region Filters
            /// <summary>
            /// Adds a specific filter to the filterstack (all of them will be executed) and adjusts the size of the sample arrays.
            /// </summary>
            /// <returns></returns>
            public AbstractSensor AddFilter(AbstractFilter filter, string name) {
                Filters ??= [];

                Filters.Add(name, filter);
                if(RawBuffer.Length < filter.SampleRange) {
                    RawBuffer.Resize(filter.SampleRange);
                    FilteredBuffer.Resize(filter.SampleRange);
                }
                return this;
            }


            /// <summary>
            /// TODO: Applies all the filters to the currently stored sensor data.
            /// </summary>
            public void ApplyFilters() {
                FilteredBuffer.Push(RawBuffer[0]);
                AbstractFilter filter;
                for(int i = 0; i < Filters.Count; i++) {
                    filter = Filters.ElementAt(i).Value;
                    filter.EvaluateConstants();
                    FilteredBuffer[0] = filter.FilterValue(RawBuffer, FilteredBuffer);
                }
            }
            #endregion Filters


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