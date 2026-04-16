namespace Arsemi {

    namespace IPC {
        public static class SerialProtocol {
            private const byte CategorySize = 32;
            public const int BaudRate = 9600;
            public const int ReceivedBytesThreshold = 1;
            public const byte PackageStartByte = 0;

            private struct Categories {
                public const byte System = CategorySize * 0;
                public const byte Setup = CategorySize * 1;
                public const byte Sensor = CategorySize * 2;
            }


            // TODO: use package for each command?
            // struct CommandStructure {
            // public:
            //   static const uint8_t Value;
            //   static const uint8_t ParameterCount;
            // };


            public struct SystemCodes {
                public const byte HibernateMicrocontroller = Categories.System + 1;
                public const byte WakeMicrocontroller = Categories.System + 2;
                public const byte SystemError = Categories.System + 3;
                public const byte RequestHandshake = Categories.System + 4;
                public const byte ReplyHandshake = Categories.System + 5;
            }


            public struct SetupCodes {
                public const byte ClearConfiguration = Categories.Setup + 1;
                public const byte AddSensor = Categories.Setup + 2;
            }


            public struct SensorCodes {
                public const byte NewSample = Categories.Sensor + 1;
            }


            /// <summary>
            /// Combines the message code and the parameters to a ready-to-send message for the serial interface with the microcontroller.
            /// TODO: rework to use numbers instead of strings
            /// </summary>
            /// <param name="code"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            // public static byte[] Serialize(Package package) {
            //     byte[] result = new byte[2 + package.Parameters.Length];
            //     result[0] = package.ActionCode;
            //     result[1] = package.Timestamp;
            //     if(package.Parameters != null) {
            //         for(int i = 0; i < package.Parameters.Length; i++) {
            //             result[2 + i] = package.Parameters[i];
            //         }
            //     }
            //     // result += End;
            //     return result;
            // }

            // public static Package Split(byte[] message) {
            //     if(!message.Contains(':')) {
            //         return new Package();
            //     }

            //     string? filteredMessage = FilterUnwantedSymbols(message);
            //     if(filteredMessage == null) {
            //         throw new Exception("Couldn't parse Message, may be corrupt: " + message);
            //     }

            //     string[] split = message.Split(Delimiter);
            //     Package result = new Package();
            //     if(!byte.TryParse(split[0], out result.Timestamp)) {
            //         throw new Exception("Couldn't parse timestamp. Message may be corrupt: " + message);
            //     }
            //     if(!byte.TryParse(split[1], out result.ActionCode)) {
            //         throw new Exception("Couldn't parse action code. Message may be corrupt: " + message);
            //     }
            //     result.Parameters = [.. split.Skip(2)];
            //     return result;
            // }


            /// <summary>
            /// Filters out unwanted symbols (currently only "?")
            /// </summary>
            /// <param name="message"></param>
            /// <returns>String containing the message without the unwanted symbols</returns>
            // private static string? FilterUnwantedSymbols(string message) {
            //     return message.Select(input => input != '?').ToString();
            // }

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