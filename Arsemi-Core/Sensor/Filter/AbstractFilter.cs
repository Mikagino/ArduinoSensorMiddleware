namespace Arsemi {
    namespace Sensor {
        namespace Filter {
            public abstract class AbstractFilter {
                public int SampleRange = 0;
                public bool Enabled = true;
                public string Name = "";
                public abstract float FilterValue(Utilities.RingBuffer samples);
            }
        }
    }
}