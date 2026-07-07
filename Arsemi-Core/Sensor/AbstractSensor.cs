using System.Text.Json.Serialization;
using Arsemi.Sensor.Event;
using Arsemi.Sensor.Filter;
using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        public class AbstractSensor : ISensor {
            [JsonInclude] public SensorData Data = new();
            [JsonInclude] public Dictionary<string, AbstractFilter> Filters = [];  // TODO: rework like events to store names into enum (can be added at compile time)
            [JsonInclude] public Dictionary<string, Predicate<RingBuffer>> Events = []; // TODO: rework like events to store names into enum (can be added at compile time)


            public Action<EventData>? EventReceived;


            protected static List<uint> _previouslyGeneratedIDs = [];


            public enum SensorTypes {
                EMPTY = 0,
                TYPE_GENERIC_ANALOG = 1,
                TYPE_GENERIC_DIGITAL = 2,
                TYPE_GENERIC_I2C = 3,
                TYPE_MAX30102 = 4,
            }
            public const SensorTypes SensorType = SensorTypes.EMPTY;


            #region Samples
            public RingBuffer RawSamples = new();
            public RingBuffer FilteredSamples = new();
            #endregion Samples


            public void PushNewValue(byte x, byte y) {
                RawSamples.Push(x, y);
                ApplyFilters();
                Data.Value = FilteredSamples[0].Y;
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
            /// Parses the sensor's data into a format for sending over serial, packaged in bytes for minimal size
            /// </summary>
            /// <returns>[SensorType, IntervalMS]</returns>
            public virtual byte[] ParseDataToByteArray() {
                return [(byte)SensorType, Data.IntervalMS];
            }


            #region Filters
            /// <summary>
            /// Adds a specific filter to the filterstack (all of them will be executed) and adjusts the size of the sample arrays.
            /// </summary>
            /// <returns></returns>
            public AbstractSensor AddFilter(AbstractFilter filter, string name) {
                Filters ??= [];

                Filters.Add(name, filter);
                if(RawSamples.Length < filter.SampleRange) {
                    RawSamples.Resize(filter.SampleRange);
                    FilteredSamples.Resize(filter.SampleRange);
                }
                return this;
            }


            /// <summary>
            /// TODO: Applies all the filters to the currently stored sensor data.
            /// <para>1. Apply each filter, sequentially according to their order in the array Filters</para>
            /// <para>2. Set FilteredSamples to the filtered values of the last filter or raw samples if no filters are applied</para>
            /// </summary>
            public void ApplyFilters() {
                RingBuffer lastSamples = RawSamples;
                AbstractFilter? filter = null;
                for(int i = 0; i < Filters.Count; i++) {
                    filter = Filters.ElementAt(i).Value;
                    filter.FilterValue(lastSamples);
                    lastSamples = filter.FilteredSamples;
                }
                FilteredSamples = filter == null ? RawSamples : filter.FilteredSamples;
            }
            #endregion Filters


            public AbstractSensor SetInterval(byte milliseconds) {
                Data.IntervalMS = milliseconds;
                return this;
            }


            #region Events
            /// <summary>
            /// Adds event to the dictionary Events
            /// </summary>
            /// <param name="name"></param>
            /// <param name="eventCondition">Condition to check against</param>
            /// <returns></returns>
            public AbstractSensor AddEvent(string name, Predicate<RingBuffer> eventCondition) {
                Events ??= [];

                Events.Add(name, (a) => eventCondition(a));
                return this;
            }


            /// <summary>
            /// Calls CheckCondition() on each event and invokes Actions if conditions are met.
            /// </summary>
            public void CheckEventsConditions() {
                foreach(var @event in Events) {
                    @event.Value.Invoke(RawSamples);
                }
            }
            #endregion Events
        }
    }
}