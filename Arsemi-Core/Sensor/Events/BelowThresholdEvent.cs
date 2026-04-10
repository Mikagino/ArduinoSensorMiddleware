using System.Text.Json.Serialization;
using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {
        namespace Event {
            public class BelowThresholdEvent : AbstractEvent {
                [JsonInclude] public float Threshold = 0;
                private bool _active = false;


                public BelowThresholdEvent(float threshold) {
                    Threshold = threshold;
                }


                public override bool CheckCondition(RingBuffer ringBuffer) {
                    if(ringBuffer[0].Y < Threshold && !_active) {
                        _active = true;
                        return true;
                    }
                    else if(ringBuffer[0].Y > Threshold && _active) {
                        _active = false;
                    }
                    return false;
                }
            }
        }
    }
}