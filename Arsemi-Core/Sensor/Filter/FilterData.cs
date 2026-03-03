using System.Runtime.InteropServices;

namespace Arsemi {
    namespace Sensor {
        namespace Filter {
        [StructLayout(LayoutKind.Sequential)]
            public struct FilterData {
                public bool Added = false;
                public float Min = 0;
                public float Max = 0;
                public float[] FilterValues = new float[5];

                public FilterData() {
                }
            }
        }
    }
}