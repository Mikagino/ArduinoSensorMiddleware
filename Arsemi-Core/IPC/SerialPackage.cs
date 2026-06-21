
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


            public byte[] Serialize(int from = 0, int to = 0) {
                byte[] result = new byte[1 + Parameters.Count];
                result[0] = ActionCode;
                if(Parameters != null) {
                    for(int i = from; i < (to == 0 ? Parameters.Count : to); i++) {
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