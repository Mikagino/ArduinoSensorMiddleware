namespace Arsemi {
    namespace Sensor {
        namespace Filter {
            public static class FilterAliases {
                /// <summary>
                /// Creates a new butterworth low pass filter for filtering out low frequencies
                /// </summary>
                /// <param name="samplingRate"></param>
                /// <param name="frequency"></param>
                /// <returns></returns>
                public static AbstractFilter ButterworthLowPassFilter(float samplingRate, float frequency) {
                    return new IIRFilter(samplingRate, frequency);
                }


                /// <summary>
                /// Creates a new butterworth high pass filter for filtering out high frequencies.
                /// <para> Based on answer by Sphinxxxx: https://stackoverflow.com/questions/8079526/lowpass-and-high-pass-filter-in-c-sharp </para>
                /// </summary>
                /// <param name="samplingRate"></param>
                /// <param name="frequency"></param>
                /// <returns></returns>
                public static AbstractFilter ButterworthHighPassFilter(float samplingRate, float frequency) {
                    float c = (float)Math.Tan(Math.PI * frequency / samplingRate);
                    float a0 = 1.0f / (1.0f + IIRFilter.Sqrt2 * c + c * c);
                    float a1 = -2f * a0;
                    float a2 = a0;
                    float b1 = 2.0f * (c * c - 1.0f) * a0;
                    float b2 = (1.0f - IIRFilter.Sqrt2 * c + c * c) * a0;
                    return new IIRFilter(a0, a1, a2, b1, b2);
                }


                /// <summary>
                /// Creates a new notch filter for either bandpass or bandreject.
                /// <para> The narrowest bandwidth that can be obtain with single precision is about 0.0003 of the sampling frequency.
                /// When pushed beyond this limit, the attenuation of the notch will degrade. </para>
                /// <para>Source: https://dspguide.com/ch19/3.htm</para>
                /// </summary>
                /// <param name="samplingRate"></param>
                /// <param name="frequency">center of the filter (as fraction of the sampling frequency from 0-1)</param>
                /// <param name="bandwidth">measured at an amplitude of 0.707</param>
                /// <returns></returns>
                public static AbstractFilter NotchBandpassBandrejectFilter(float samplingRate, float frequency, float bandwidth) {
                    return new IIRFilter(samplingRate, frequency, bandwidth);
                }
            }
        }
    }
}