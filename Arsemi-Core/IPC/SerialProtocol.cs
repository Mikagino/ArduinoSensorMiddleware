namespace Arsemi {

    namespace IPC {
        public static class SerialProtocol {

            public struct Package() {
                public uint Timestamp = 0;
                public uint ActionCode = 0;
                public string[] Parameters = [];


                public Package(uint timestamp, uint actionCode, string[] parameters) : this() {
                    Timestamp = timestamp;
                    ActionCode = actionCode;
                    Parameters = parameters;
                }
            }


            private const string Delimiter = ":";
            // private const string End = "|";
            private const int CodeShifter = 100;
            public const int BaudRate = 9600;


            private struct Categories {
                public const uint System = 1;
                public const uint Setup = 2;
                public const uint Sensor = 3;
            }


            public struct SystemCodes {
                public const uint HibernateMicrocontroller = (Categories.System * CodeShifter) + 1;
                public const uint WakeMicrocontroller = (Categories.System * CodeShifter) + 2;
                public const uint SystemError = (Categories.System * CodeShifter) + 3;
                public const uint RequestHandshake = (Categories.System * CodeShifter) + 4;
                public const uint ReplyHandshake = (Categories.System * CodeShifter) + 5;
            }


            public struct SetupCodes {
                public const uint ClearConfiguration = (Categories.Setup * CodeShifter) + 1;
                public const uint AddSensor = (Categories.Setup * CodeShifter) + 2;
            }


            public struct SensorCodes {
                public const uint NewSample = (Categories.Sensor * CodeShifter) + 1;
            }


            /// <summary>
            /// Combines the message code and the parameters to a ready-to-send message for the serial interface with the microcontroller.
            /// TODO: rework to use numbers instead of strings
            /// </summary>
            /// <param name="code"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public static string CombineToMessage(long timestamp, uint code, params string[] parameters) {
                string result = timestamp + Delimiter + code;
                if(parameters != null) {
                    foreach(string arg in parameters) {
                        result += Delimiter + arg;
                    }
                }
                // result += End;
                return result;
            }

            public static Package Split(string message) {
                if(!message.Contains(':')) {
                    return new Package();
                }

                string? filteredMessage = FilterUnwantedSymbols(message);
                if(filteredMessage == null) {
                    throw new Exception("Couldn't parse Message, may be corrupt: " + message);
                }

                string[] split = message.Split(Delimiter);
                Package result = new Package();
                if(!uint.TryParse(split[0], out result.Timestamp)) {
                    throw new Exception("Couldn't parse timestamp. Message may be corrupt: " + message);
                }
                if(!uint.TryParse(split[1], out result.ActionCode)) {
                    throw new Exception("Couldn't parse action code. Message may be corrupt: " + message);
                }
                result.Parameters = [.. split.Skip(2)];
                return result;
            }


            /// <summary>
            /// Filters out unwanted symbols (currently only "?")
            /// </summary>
            /// <param name="message"></param>
            /// <returns>String containing the message without the unwanted symbols</returns>
            private static string? FilterUnwantedSymbols(string message) {
                return message.Select(input => input != '?').ToString();
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