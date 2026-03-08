using System.Numerics;

namespace Arsemi {
    namespace Sensor {
        namespace Filter {
            public abstract class AbstractFilter {
                public virtual int SampleRange => 0;
                public bool Enabled = true;
                public string Name = "";
                public abstract void EvaluateConstants();
                public abstract Vector2 FilterValue(Utilities.RingBuffer rawSamples, Utilities.RingBuffer filteredSamples);
            }
        }
    }
}