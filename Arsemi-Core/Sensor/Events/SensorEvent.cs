using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        namespace Event {
            public class SensorEvent {
                private Action? _action;
                private EventCondition _eventCondition;

                /// <summary>
                /// Creates a new instance of AbstractEvent which is for invoking the action when the condition of the subclass is met
                /// </summary>
                /// <param name="action">Action which will be invoked when the event condition is met</param>
                public SensorEvent(Action action, EventCondition eventCondition) {
                    _action = action;
                    _eventCondition = eventCondition;
                }


                /// <summary>
                /// Invokes the action supplied in the constructor
                /// </summary>
                /// <param name="ringBuffer"></param>
                public void CheckCondition(RingBuffer ringBuffer) {
                    if(_eventCondition.CheckCondition(ringBuffer)) _action?.Invoke();
                }
            }
        }
    }
}