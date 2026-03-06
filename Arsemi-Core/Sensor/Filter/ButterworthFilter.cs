using System.Text.Json.Serialization;

namespace Arsemi {
    namespace Sensor {
        namespace Filter {
            /// <summary>
            /// Implementation of the Butterworth Filter based on:
            /// A Butterworth Filter in C# - Darryl Bryk
            /// </summary>
            public class ButterworthFilter : AbstractFilter {

                #region Public Properties
                [JsonInclude] public float CutOff {
                    set {
                        _cutOff = value;
                        EvaluateConstants();
                    }
                    get => _cutOff;
                }
                private float _cutOff;

                private readonly AbstractSensor _sensor;
                #endregion Public Properties


                #region Evaluated Properties
                private const float Sqrt2 = 1.414213562f;
                private float _a0, _a1, _a2;
                private float _b1, _b2;

                private float _x1, _x2;
                private float _y1, _y2;
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

                    _x1 = _x2 = _y1 = _y2 = 0;
                }


                /// <summary>
                /// </summary>
                /// <param name="input"></param>
                /// <returns>Filtered value based on the previously evaluated properties and older stored values.</returns>
                public override float FilterValue(float input) {
                    float y = (_a0 * input) + (_a1 * _x1) + (_a2 * _x2) + (_b1 * _y1) + (_b2 * _y2);

                    _x2 = _x1;
                    _x1 = input;
                    _y2 = _y1;
                    _y1 = y;

                    return y;
                }
            }
        }
    }
}