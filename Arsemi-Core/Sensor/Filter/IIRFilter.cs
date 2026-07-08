using Arsemi.Utilities;

namespace Arsemi {
    namespace Sensor {

        namespace Filter {
            /// <summary>
            /// Implementation of the Butterworth Filter based on:
            /// Unity-IIR-Realtime-Filtering (mariusrubo)
            /// <para>Source: https://github.com/mariusrubo/Unity-IIR-Realtime-Filtering/blob/master/FilterData.cs</para>
            /// </summary>
            public class IIRFilter : AbstractFilter {

                /// <summary>
                /// Values used for applying filter, calculated once at filter creation.
                /// </summary>
                private readonly float _a0;
                private readonly float _a1;
                private readonly float _a2;
                private readonly float _b1;
                private readonly float _b2;
                public const float Sqrt2 = 1.414213562f;



                /// <summary>
                /// two parameters indicate a 2nd order Butterworth low-pass filter
                /// equation obtained here: https://www.codeproject.com/Tips/1092012/A-Butterworth-Filter-in-Csharp
                /// note that you can also use the five-parameter-solution described below. This is just for convenience. 
                /// </summary>
                /// <param name="samplingRate"></param>
                /// <param name="frequency"></param>
                public IIRFilter(float samplingRate, float frequency) {
                    float wc = MathF.Tan(frequency * MathF.PI / samplingRate);
                    float k1 = Sqrt2 * wc;
                    float k2 = wc * wc;
                    _a0 = k2 / (1 + k1 + k2);
                    _a1 = 2 * _a0;
                    _a2 = _a0;
                    float k3 = _a1 / k2;
                    _b1 = -2 * _a0 + k3;
                    _b2 = 1 - (2 * _a0) - k3;

                    FilteredSamples.Reset();
                }


                /// <summary>
                /// three parameters indicates a notch filter
                /// equation obtained here http://dspguide.com/ch19/3.htm
                /// </summary>
                /// <param name="samplingRate"></param>
                /// <param name="frequency"></param>
                /// <param name="bandwidth"></param>
                public IIRFilter(float samplingRate, float frequency, float bandwidth) {
                    float f = frequency / samplingRate;
                    float r = 1 - 3 * (bandwidth / samplingRate);
                    _a0 = (1 - 2 * r * MathF.Cos(MathF.Tau * f) + r * r) / (2 - 2 * MathF.Cos(MathF.Tau * f));
                    _a1 = -2 * _a0 * MathF.Cos(MathF.Tau * f);
                    _a2 = _a0;
                    _b1 = 2 * r * MathF.Cos(MathF.Tau * f);
                    _b2 = -r * r;
                }


                /// <summary>
                /// Five parameters indicate a generic filter. 
                /// This is necessary for Butterworth high-pass filters or other IIR filters (because I could not implement the process of obtaining these parameters in here).  
                /// Easy way to find these parameters is using R's "signal" package butter function, and convert parameters like this: 
                /// <para>samplingRate: 500 (in Hz)</para>
                /// <para>cutoff: 1 (in Hz)</para>
                /// <para>order: 2</para>
                /// <para>(nyquist: samplingRate/2) </para>
                /// <para>W: cutoff/nyquist </para>
                /// <para>bf: signal::butter(order, W, type = "high")
                ///     a0: bf$b[1] / bf$a[1]
                ///     a1: bf$b[2] / bf$a[1]
                ///     a2: bf$b[3] / bf$a[1]
                ///     b1: -bf$a[2] / bf$a[1]
                ///     b2: -bf$a[3] / bf$a[1] </para>
                /// paste0("new IIRFilter(",a0, "f, " ,a1, "f, ", a2, "f, ", b1, "f, ", b2, "f);") 
                /// </summary>
                /// <param name="a0in"></param>
                /// <param name="a1in"></param>
                /// <param name="a2in"></param>
                /// <param name="b1in"></param>
                /// <param name="b2in"></param>
                public IIRFilter(float a0in, float a1in, float a2in, float b1in, float b2in) {
                    _a0 = a0in;
                    _a1 = a1in;
                    _a2 = a2in;
                    _b1 = b1in;
                    _b2 = b2in;
                }


                /// <summary>
                /// filter data. Each IIRFilter stores two data points of filtered and unfiltered data. Therefore, filtering should be continuous and not be switched on and off. 
                /// Furthermore, each IIRFilter may only process one data stream. If you intend to filter two data streams with the same kind of filter, you need to initialize 
                /// two IIRFilters accordingly (e.g. "Notch50_1" and "Notch50_2"), each filtering only one data stream. (by mariusrobo)
                /// 
                /// <para>
                /// 2 datapoints of the filtered data are stored in each filter. For applying multiple filter, the next filter should use the filtered values of the last filter. (by mikagino)
                /// </para>
                /// 
                /// </summary>
                /// <param name="rawSamples"></param>
                public override void FilterValue(RingBuffer rawSamples) {
                    byte y = (byte)(_a0 * rawSamples[0].Y +
                                    _a1 * rawSamples[1].Y +
                                    _a2 * rawSamples[2].Y +
                                    _b1 * FilteredSamples[0].Y +
                                    _b2 * FilteredSamples[1].Y);

                    FilteredSamples.Push(rawSamples[0].X, y);
                }
            }
        }
    }
}