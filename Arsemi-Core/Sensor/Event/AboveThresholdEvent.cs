using System.Text.Json.Serialization;
using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        namespace Event {
            public class AboveThresholdEvent : EventCondition {
                [JsonInclude] public float Threshold = 0;
                private bool _active = false;


                public AboveThresholdEvent(float threshold, Action action) {
                    Threshold = threshold;
                    Action = action;
                }


                public override void CheckCondition(RingBuffer ringBuffer) {
                    if(ringBuffer[0].Y > Threshold && !_active) {
                        _active = true;
                        Action?.Invoke();
                    }
                    else if(ringBuffer[0].Y < Threshold && _active) {
                        _active = false;
                    }
                }
            }
        }
    }
}