namespace Arsemi {
    namespace Sensor {
        namespace Filter {
            public abstract class AbstractFilter {
                public FilterData Data;
                
                public abstract float FilterValue(float input);
            }
        }
    }
}