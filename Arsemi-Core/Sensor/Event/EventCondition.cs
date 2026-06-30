using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        namespace Event {
            /// <summary>
            /// Abstract class for checking if a sensor value meets a certain state. Can be overriden to make custom checks
            /// (Be cautious with making your own overrides of this class because the checks are done every time a new data sample arrives).
            /// </summary>
            public abstract class EventCondition {
                public string Name = "EventCondition";
                public Action? Action;

                /// <summary>
                /// Checks if the condition of the subclass is met and invokes the action (set in the constructor) if the check returns true
                /// </summary>
                /// <param name="ringBuffer">The buffer on which the values are checked</param>
                public abstract void CheckCondition(RingBuffer ringBuffer);
            }
        }
    }
}