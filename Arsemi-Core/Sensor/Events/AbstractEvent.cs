using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        namespace Event {
            public abstract class AbstractEvent {
                /// <summary>
                /// </summary>
                /// <param name="ringBuffer"></param>
                /// <returns>true if the event's condition is met</returns>
                public abstract bool CheckCondition(RingBuffer ringBuffer);
            }
        }
    }
}