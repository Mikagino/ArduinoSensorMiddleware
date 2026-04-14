using System.Text.Json.Serialization;
using Arsemi.Sensor.Event;
using Arsemi.Sensor.Filter;
using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            [JsonInclude] public SensorData Data = new();
            [JsonInclude] public Dictionary<string, AbstractFilter> Filters = [];
            [JsonInclude] public Dictionary<string, AbstractEvent> Events = [];


            protected static List<uint> _previouslyGeneratedIDs = [];


            #region Samples
            public RingBuffer RawBuffer = new();
            public RingBuffer FilteredBuffer = new();
            #endregion Samples


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


            public virtual string[] GetDataAsStrings() {
                return [Data.ID.ToString(), Data.IntervalMS.ToString()];
            }


            public static string ParseSensorIdToName(uint sensorId) {
                return ((ArsemiGlobals.Sensors)sensorId).ToString();
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


            #region Events
            /// <summary>
            /// Adds event to the dictionary Events
            /// </summary>
            public AbstractSensor AddEvent(AbstractEvent eventType, string name) {
                Events ??= [];

                Events.Add(name, eventType);
                return this;
            }


            /// <summary>
            /// Calls CheckCondition() on each event and invokes Actions if conditions are met
            /// </summary>
            public void CheckEventsConditions() {
                foreach(var @event in Events) {
                    if(@event.Value.CheckCondition(RawBuffer)) {
                        ArsemiGlobals.Events.EventMap[@event.Key]()?.Invoke();
                    }
                }
            }
            #endregion Events
        }
    }
}