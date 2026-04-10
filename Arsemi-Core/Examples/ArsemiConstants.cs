using System.Reflection;

namespace ArsemiConstants {
    /// <summary>
    /// This file will be generated upon calling -INSERT METHOD- or when using the GUI for generating the configuration.
    /// </summary>
    public enum Sensors {
        Heartrate,
        Button,
    }

    public class Actions {
        public static Action? Excitement;


        public static Dictionary<string, Func<Action?>> ActionMap = new(
            [
                new("Excitement", () => Excitement),
            ]
        );
    }
}