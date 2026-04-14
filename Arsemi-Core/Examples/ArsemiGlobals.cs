namespace ArsemiGlobals {
    /// <summary>
    /// This file will be generated upon calling -INSERT METHOD- or when using the GUI for generating the configuration.
    /// </summary>
    public enum Sensors {
        Heartrate,
        GSR,
    }

    public class Events {
        public static Action? Excitement;


        public static Dictionary<string, Func<Action?>> EventMap = new(
            [
                new("Excitement", () => Excitement),
            ]
        );
    }
}