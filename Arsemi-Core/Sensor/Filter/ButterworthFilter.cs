using System.Numerics;
using System.Text.Json.Serialization;

namespace Arsemi {
    namespace Sensor {
        namespace Filter {
            /// <summary>
            /// Implementation of the Butterworth Filter based on:
            /// >A Butterworth Filter in C#<(Darryl Bryk) and >Unity-IIR-Realtime-Filtering<(mariusrubo)
            /// </summary>
            public class ButterworthFilter : AbstractFilter {

                #region Public Properties
                [JsonInclude]
                public float CutOff {
                    set {
                        _cutOff = value;
                        EvaluateConstants();
                    }
                    get => _cutOff;
                }
                private float _cutOff;

                private readonly AbstractSensor _sensor;
                public new int SampleRange = 2;
                #endregion Public Properties


                #region Evaluated Properties
                private const float Sqrt2 = 1.414213562f;
                private float _a0, _a1, _a2;
                private float _b1, _b2;

                #endregion Evaluated Properties


                public ButterworthFilter(AbstractSensor sensor, float cutOff = 10) {
                    _sensor = sensor;
                    CutOff = cutOff;
                }


                /// <summary>
                /// Recalculates values for FilterValues(). Only called when important values change to improve performance.
                /// </summary>
                public void EvaluateConstants() {
                    if(_cutOff == 0) {
                        return;
                    }

                    float samplingRate = 1f / _sensor.Data.IntervalMS;
                    float wc = MathF.Tan(CutOff * MathF.PI / samplingRate);
                    float k1 = Sqrt2 * wc;
                    float k2 = wc * wc;

                    _a0 = k2 / (1 + k1 + k2);
                    _a1 = 2f * _a0;
                    _a2 = _a0;
                    float k3 = _a1 / k2;
                    _b1 = -2f * _a0 + k3;
                    _b2 = 1f - (2f * _a0) - k3;
                }


                /// <summary>
                /// </summary>
                /// <param name="input"></param>
                /// <returns>Filtered value based on the previously evaluated properties and older stored values.</returns>
                public override Vector2 FilterValue(Utilities.RingBuffer samples) {
                    return new() {
                        Y = (_a0 * samples[0].X)
                        + (_a1 * samples[1].X)
                        + (_a2 * samples[2].X)
                        + (_b1 * samples[1].Y)
                        + (_b2 * samples[2].Y),
                        X = samples[0].X
                    };
                }
            }
        }
    }
}