namespace Arsemi {

    namespace IPC {

        public class SerialPackage() {
            public byte ActionCode = 0;
            public byte[] Parameters = [];


            public SerialPackage(byte actionCode, params byte[] parameters) : this() {
                ActionCode = actionCode;
                Parameters = parameters;
            }


            public byte[] Serialize() {
                byte[] result = new byte[1 + Parameters.Length];
                result[0] = ActionCode;
                if(Parameters != null) {
                    for(int i = 0; i < Parameters.Length; i++) {
                        result[1 + i] = Parameters[i];
                    }
                }
                return result;
            }
        }
    }
}