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

            public static class Action {
                public struct System {
                    public const byte HibernateMicrocontroller = Categories.System + 1;
                    public const byte WakeMicrocontroller = Categories.System + 2;
                    public const byte SystemError = Categories.System + 3;
                    public const byte RequestHandshake = Categories.System + 4;
                    public const byte ReplyHandshake = Categories.System + 5;
                }


                public struct Setup {
                    public const byte ClearConfiguration = Categories.Setup + 1;
                    public const byte AddSensor = Categories.Setup + 2;
                    public const byte SuccessfullyAddedSensor = Categories.Setup + 3;
                }


                public struct Sensor {
                    public const byte NewSample = Categories.Sensor + 1;
                }
            }


            public static string? TryGetActionName(uint actionCode) {
                string? result = TryGetActionFromStruct(typeof(Action.System), actionCode);
                result ??= TryGetActionFromStruct(typeof(Action.Setup), actionCode);
                result ??= TryGetActionFromStruct(typeof(Action.Sensor), actionCode);
                return result;
            }

            private static string? TryGetActionFromStruct(Type type, uint actionCode) {
                foreach(var property in type.GetFields()) {
                    byte? value = (byte?)property.GetRawConstantValue();
                    if(value == null) throw new Exception("Not found in " + type.Name);
                    if(value == actionCode) {
                        return property.Name;
                    }
                }
                return null;
            }
        }
    }
}