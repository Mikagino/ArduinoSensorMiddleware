
namespace Arsemi {

    namespace IPC {

        public class SerialPackage() {
            public byte ActionCode {
                set {
                    Empty = false;
                    _actionCode = value;
                }
                get => _actionCode;
            }
            private byte _actionCode;

            public List<byte> Parameters = [];
            public byte Crc8;
            public bool Empty = true;
            public bool Done = false;



            public SerialPackage(byte actionCode, params byte[] parameters) : this() {
                ActionCode = actionCode;
                Parameters = [.. parameters];
            }

            public void AppendParameter(byte value) {
                Parameters.Add(value);
                Empty = false;
            }

            /// <summary>
            /// Serializes the package into an array of bytes. Layout: [Action Code | Parameters]. Won't contain the Crc8.
            /// </summary>
            /// <param name="from">Inclusive start from which index to serialize. Keep at 0 if you want to serialize from the beginning.</param>
            /// <param name="to">Exclusive end to which index to serialize. Keep at 0 if you want to serialize until the end.</param>
            /// <returns>Byte array of the serialized package</returns>
            public byte[] Serialize(int from = 0, int to = 0) {
                if(to == 0) to = Parameters.Count + 1;
                byte[] result = new byte[to - from];
                if(from == 0) result[0] = ActionCode;
                if(Parameters != null) {
                    for(int i = from; i < Math.Min(to - 1, Parameters.Count); i++) {
                        result[1 + i] = Parameters[i];
                    }
                }
                return result;
            }

            public void Reset() {
                Done = false;
                Empty = true;
                Parameters = [];
            }
        }
    }
}