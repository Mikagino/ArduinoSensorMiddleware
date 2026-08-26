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
            public class HeartrateAnalysis(int sampleCount) {
                #region Constants
                private const float RThreshold = 0.7f;
                private const float DecayRate = 0.02f;
                private const float ThresholdRate = 0.05f;
                private const int MinDiff = 50;
                #endregion Constants


                #region Current values
                private float _maxValue = 0;
                private float _minValue = 0;
                private float _threshold = 0;
                private long _lastHeartbeatMillis = 0;
                private float _lastValue = 0;
                #endregion Current values

                private Stopwatch _watch = new();


                public void CheckForBeat(byte currentValue) {
                    if(!_watch.IsRunning) _watch.Start();
                    
                    _maxValue = MathF.Max(_maxValue, currentValue);
                    _minValue = MathF.Min(_minValue, currentValue);

                    float nthreshold = (_maxValue - _minValue) * RThreshold + _minValue;
                    _threshold = _threshold * (1 - ThresholdRate) + nthreshold * ThresholdRate;
                    _threshold = MathF.Min(_maxValue, MathF.Min(_minValue, _threshold));

                    if(currentValue >= _threshold
                        && _lastValue < _threshold
                        && (_maxValue - _minValue) > MinDiff
                        && _watch.ElapsedMilliseconds - _lastHeartbeatMillis > 300) {
                        if(_lastHeartbeatMillis != 0) {
                            // Show Results
                            int bpm = (int)(60000 / (_watch.ElapsedMilliseconds - _lastHeartbeatMillis));
                            if(bpm > 50 && bpm < 250) {
                                Console.Write("Heart Rate (bpm): ");
                                Console.WriteLine(bpm);
                            }
                        }
                        _lastHeartbeatMillis = _watch.ElapsedMilliseconds;
                    }
                    _maxValue -= (_maxValue - currentValue) * DecayRate;
                    _minValue += (currentValue - _minValue) * DecayRate;
                    _lastValue = currentValue;
                }
            }
        }
    }
}