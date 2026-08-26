using System.Diagnostics;

namespace Arsemi {
    namespace Sensor {
        namespace Analysis {
            /// <summary>
            /// Analyse heartrate sensor samples to extract, bpm, variance and others.
            /// Adapted from:
            /// <see cref="https://devxplained.eu/en/blog/max3010x-pulse-oximeter-modules-part-2"/>
            /// <para>TODO: implement more complex, adaptive algorithm.</para>
            /// </summary>
            /// <param name="sampleCount"></param>
            public class HeartrateAnalysis {
                #region Constants
                private const float RThreshold = 0.7f;
                private const float DecayRate = 0.02f;
                private const float ThresholdRate = 0.05f;
                private const int MinDiff = 50;
                #endregion Constants


                #region Computed values
                private float _maxValue = 0;
                private float _minValue = 0;
                private float _threshold = 0;
                #endregion Computed values


                #region Current values
                private long _lastHeartbeatMillis = 0;
                private float _lastValue = 0;

                #endregion Current values

                private Stopwatch _watch = new();


                public Action? HeartbeatDetected;


                /// <summary>
                /// Computes some values based on current value and emits HeartbeatDetected if applicable.
                /// </summary>
                /// <param name="currentValue"></param>
                public void CheckForBeat(byte currentValue) {
                    if(!_watch.IsRunning) _watch.Start();

                    ComputeValues(currentValue);

                    if(HeartbeatReachedPeak(currentValue)) {
                        InvokeHeartbeatFeedback();
                    }

                    FinalizeEvaluation(currentValue);
                }


                /// <summary>
                /// Computes the necessary values for the evaluation of a heartbeat
                /// </summary>
                /// <param name="currentValue"></param>
                private void ComputeValues(byte currentValue) {
                    _maxValue = MathF.Max(_maxValue, currentValue);
                    _minValue = MathF.Min(_minValue, currentValue);

                    float nthreshold = (_maxValue - _minValue) * RThreshold + _minValue;
                    _threshold = _threshold * (1 - ThresholdRate) + nthreshold * ThresholdRate;
                    _threshold = MathF.Min(_maxValue, MathF.Min(_minValue, _threshold));
                }


                /// <summary>
                /// Check naively if the current value reached the peak of a heartbeat.
                /// </summary>
                /// <param name="currentValue"></param>
                /// <returns></returns>
                private bool HeartbeatReachedPeak(byte currentValue) {
                    return currentValue >= _threshold
                        && _lastValue < _threshold
                        && (_maxValue - _minValue) > MinDiff
                        && _watch.ElapsedMilliseconds - _lastHeartbeatMillis > 300;
                }


                /// <summary>
                /// Prints out bpm, invokes HeartbeatDetected and finalizes bpm detection.
                /// </summary>
                private void InvokeHeartbeatFeedback() {
                    if(_lastHeartbeatMillis != 0) {
                        int bpm = (int)(60000 / (_watch.ElapsedMilliseconds - _lastHeartbeatMillis));
                        if(bpm > 50 && bpm < 250) {
                            Console.Write("Heart Rate (bpm): ");
                            Console.WriteLine(bpm);
                        }
                    }
                    HeartbeatDetected?.Invoke();
                    _lastHeartbeatMillis = _watch.ElapsedMilliseconds;
                }


                /// <summary>
                /// Computes max, min and sets lastValue for later cycles
                /// </summary>
                /// <param name="currentValue"></param>
                private void FinalizeEvaluation(byte currentValue) {
                    _maxValue -= (_maxValue - currentValue) * DecayRate;
                    _minValue += (currentValue - _minValue) * DecayRate;

                    _lastValue = currentValue;
                }
            }
        }
    }
}