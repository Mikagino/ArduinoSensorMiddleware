namespace Arsemi {

    namespace IPC {
        public static class SerialProtocol {
            private const byte CategorySize = 32;
            public const int BaudRate = 9600;
            public const int ReceivedBytesThreshold = 3;
            public const byte PackageStartByte = 0;

            private struct Categories {
                public const byte System = CategorySize * 0;
                public const byte Setup = CategorySize * 1;
                public const byte Sensor = CategorySize * 2;
                public const byte Package = CategorySize * 3;
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
                    public const byte Error = Categories.System + 3;
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

            public class Error {
                public struct Package {
                    public const byte InvalidActionCode = Categories.Package + 1;
                    public const byte InvalidSensorParameters = Categories.Package + 2;
                    public const byte SensorCountOverflow = Categories.Package + 3;
                    public const byte InvalidSensorType = Categories.Package + 4;
                    public const byte PackageSizeOverflow = Categories.Package + 5;
                    public const byte InvalidChecksum = Categories.Package + 6;
                }
            }


            public static string? TryGetActionName(uint actionCode) {
                string? result = TryGetNameFromStruct(typeof(Action.System), actionCode);
                result ??= TryGetNameFromStruct(typeof(Action.Setup), actionCode);
                result ??= TryGetNameFromStruct(typeof(Action.Sensor), actionCode);
                return result?? "?";
            }


            public static string? TryGetErrorName(uint errorCode) {
                string? result = TryGetNameFromStruct(typeof(Error.Package), errorCode);
                return result ?? "?";
            }

            private static string? TryGetNameFromStruct(Type type, uint actionCode) {
                foreach(var property in type.GetFields()) {
                    byte? value = (byte?)property.GetRawConstantValue() ?? throw new Exception("Not found in " + type.Name);
                    if(value == actionCode) {
                        return property.Name;
                    }
                }
                return null;
            }
        }
    }
}