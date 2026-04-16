using System.Numerics;

namespace Arsemi {
    namespace Utilities {
        /// <summary>
        /// Flexibly sized buffer for managing Vector2D data sorted by newest to oldest from first to last index.
        /// </summary>
        public class RingBuffer {
            public class Vector2_8bit(byte x, byte y) {
                public byte X = x;
                public byte Y = y;
            }


            public Vector2_8bit this[int index] {
                get => _buffer[CycleRingIndex(index)];
                set => _buffer[CycleRingIndex(index)] = value;
            }
            public int Length => _buffer.Length;

            #region buffer handling
            private Vector2_8bit[] _buffer = [];
            private int _currentIndex = 0;
            #endregion buffer handling


            public RingBuffer() {

            }


            /// <summary>
            /// Copy constructor that copies all data from source to target.
            /// </summary>
            /// <param name="previousBuffer"></param>
            public RingBuffer(RingBuffer previousBuffer) {
                _buffer = previousBuffer._buffer;
                _currentIndex = previousBuffer._currentIndex;
            }


            public static int PosMod(int a, int m) {
                int result = a % m;
                result = result < 0 ? result + m : result;
                return result;
            }


            /// <summary>
            /// Positive modulo index for the buffer starting from the last saved value at 0.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            private int CycleRingIndex(int index) {
                if(_buffer.Length == 0) { return 0; }
                return PosMod(_currentIndex + index, _buffer.Length);
            }


            /// <summary>
            /// Resize the buffer to newSize and copy data.
            /// </summary>
            /// <param name="newSize"></param>
            public void Resize(int newSize) {
                Vector2_8bit[] newBuffer = new Vector2_8bit[newSize];
                int copyCount = Math.Min(newSize, Length);
                for(int i = 0; i < copyCount; i++) {
                    newBuffer[i] = this[i];
                }
                _buffer = newBuffer;
                _currentIndex = 0;
            }


            /// <summary>
            /// Sets the X value at the specified index in the ring buffer, starting from the current value at index 0.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="value"></param>
            public void SetX(int index, byte value) {
                int ringIndex = CycleRingIndex(index);
                _buffer[ringIndex].X = value;
            }


            /// <summary>
            /// Sets the Y value at the specified index in the ring buffer, starting from the current value at index 0.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="value"></param>
            public void SetY(int index, byte value) {
                int ringIndex = CycleRingIndex(index);
                _buffer[ringIndex].Y = value;
            }


            /// <summary>
            /// Pushes a new value into the buffer while keeping the size and the x last values, where x is Length-1.
            /// </summary>
            /// <param name="newValue"></param>
            public void Push(byte x, byte y) {
                _currentIndex = CycleRingIndex(-1);
                _buffer[_currentIndex] = new Vector2_8bit(x, y);
            }


            /// <summary>
            /// Pushes a new value into the buffer while keeping the size and the x last values, where x is Length-1.
            /// </summary>
            /// <param name="newValue"></param>
            public void Push(Vector2_8bit vector2_8Bit) {
                _currentIndex = CycleRingIndex(-1);
                _buffer[_currentIndex] = vector2_8Bit;
            }


            /// <summary>
            /// Increases only the index without providing a new value.
            /// </summary>
            /// <param name="offset"></param>
            public void MoveIndex(int offset = 0) => _currentIndex = CycleRingIndex(offset);
        }
    }
}