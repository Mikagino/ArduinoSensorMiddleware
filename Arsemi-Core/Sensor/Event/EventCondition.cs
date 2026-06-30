using System.Text.Json.Serialization;
using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        namespace Event {
            /// <summary>
            /// Abstract class for checking if a sensor value meets a certain state. Can be overriden to make custom checks
            /// (Be cautious with making your own overrides of this class because the checks are done every time a new data sample arrives).
            /// </summary>
            public static class EventCondition {
                /// <summary>
                /// Checks if the condition of the subclass is met and invokes the action (set in the constructor) if the check returns true
                /// </summary>
                /// <param name="ringBuffer"></param>
                /// <param name="threshold"></param>
                public static bool BelowThreshold(RingBuffer ringBuffer, int threshold) {
                    return ringBuffer[0].Y < threshold && ringBuffer[1].Y > threshold;
                }


                /// <summary>
                /// </summary>
                /// <param name="ringBuffer"></param>
                /// <param name="threshold"></param>
                /// <returns>true if if the current value is bigger and the previous value is lower than the treshold</returns>
                public static bool AboveThreshold(RingBuffer ringBuffer, int threshold) {
                    return ringBuffer[0].Y > threshold && ringBuffer[1].Y < threshold;
                }
            }
        }
    }
}