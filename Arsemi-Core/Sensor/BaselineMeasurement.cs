namespace Arsemi {
    namespace Sensor {
        /// <summary>
        /// Baseline measurement object meant for measuring a certain
        /// </summary>
        /// <param name="average"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public class BaselineMeasurement(float average = 0, float min = 0, float max = 0) {
            public float Average = average;
            public float Min = min;
            public float Max = max;


            public enum MeasurementState {
                PROCESSING,
                DONE,
                EMPTY,
            }
            public MeasurementState State = MeasurementState.EMPTY;


            private int _pushedValueCount = 0;
            private ulong _pushedSum = 0;


            /// <summary>
            /// Computes value into the baseline measurement (average, min, max)
            /// </summary>
            /// <param name="value"></param>
            public void ComputeValueIntoBaseline(byte value) {
                _pushedSum += value;
                _pushedValueCount++;
                Average = (long)_pushedSum / _pushedValueCount;
                if(value < Min)
                    Min = value;
                if(value > Max)
                    Max = value;
            }


            /// <summary>
            /// Sets the state of the baseline measurement to PROCESSING
            /// </summary>
            public void Start() {
                State = MeasurementState.PROCESSING;
            }


            /// <summary>
            /// Sets the state of the baseline measurement to DONE
            /// </summary>
            public void Finish() {
                State = MeasurementState.DONE;
            }
        }
    }
}