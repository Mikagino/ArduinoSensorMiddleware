using System.Text.Json.Serialization;
using Arsemi.Sensor.Filter;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            [JsonInclude] public SensorData Data = new();
            [JsonInclude] public Dictionary<string, AbstractFilter> Filters = [];


            protected static List<uint> _previouslyGeneratedIDs = [];


            #region Samples
            protected Utilities.RingBuffer _rawBuffer = new();
            protected Utilities.RingBuffer _filteredBuffer = new();
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
                if(_rawBuffer.Length < filter.SampleRange) {
                    _rawBuffer.Resize(filter.SampleRange);
                    _filteredBuffer.Resize(filter.SampleRange);
                }
                return this;
            }


            /// <summary>
            /// TODO: Applies all the filters to the currently stored sensor data.
            /// </summary>
            public void ApplyFilters() {
                for(int i = 0; i < Filters.Count; i++) {
                    _filteredBuffer[0] = Filters.ElementAt(i).Value.FilterValue(_rawBuffer);
                }

                _filteredBuffer.SetX(0, _filteredBuffer[0].X + 1);
                _filteredBuffer.MoveIndex();
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