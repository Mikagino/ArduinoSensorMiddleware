namespace Arsemi {

    namespace IPC {
        public static class SerialProtocol {
            private const string Delimiter = ":";
            private const string End = "|";
            private const int CodeShifter = 100;
            public const int BaudRate = 9600;


            private struct Categories {
                public const uint System = 0;
                public const uint Setup = 1;
                public const uint Sensor = 2;
            }


            public struct SystemCodes {
                public const uint HibernateMicrocontroller = (Categories.System * CodeShifter) + 0;
                public const uint WakeMicrocontroller = (Categories.System * CodeShifter) + 1;
                public const uint SystemError = (Categories.System * CodeShifter) + 2;
                public const uint RequestHandshake = (Categories.System * CodeShifter) + 3;
                public const uint ReplyHandshake = (Categories.System * CodeShifter) + 4;
            }


            public struct SetupCodes {
                public const uint ClearConfiguration = (Categories.Setup * CodeShifter) + 0;
                public const uint AddSensor = (Categories.Setup * CodeShifter) + 1;
            }


            public struct SensorCodes {
                public const uint SensorError = (Categories.Sensor * CodeShifter) + 0;
                public const uint NewSample = (Categories.Sensor * CodeShifter) + 1;
            }


            /// <summary>
            /// Combines the message code and the parameters to a ready-to-send message for the serial interface with the microcontroller.
            /// </summary>
            /// <param name="code"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public static string CombineToMessage(uint code, params string[] parameters) {
                string result = "0" + Delimiter + code;
                if(parameters != null) {
                    foreach(string arg in parameters) {
                        result += Delimiter + arg;
                    }
                }
                result += End;
                return result;
            }

            // ############## Maybe add the following functions later? ##############

            // /// <summary>
            // /// </summary>
            // /// <returns>Ready-to-send message for hibernation of the microcontroller.</returns>
            // public static string HibernateMicrocontroller() {
            //     return Combine(SystemCodes.HibernateMicrocontroller);
            // }


            // /// <summary>
            // /// </summary>
            // /// <returns>Ready-to-send message for hibernation of the microcontroller.</returns>
            // public static string WakeMicrocontroller(float duration) {
            //     return Combine(SystemCodes.WakeMicrocontroller, duration.ToString());
            // }


            // /// <summary>
            // /// </summary>
            // /// <returns>Ready-to-send message for hibernation of the microcontroller.</returns>
            // public static string SystemError(string message) {
            //     return Combine(SystemCodes.WakeMicrocontroller, message);
        }
    }
}