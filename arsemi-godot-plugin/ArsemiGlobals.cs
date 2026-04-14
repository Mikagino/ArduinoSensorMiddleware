
/// <summary>
/// This file will be generated upon calling GenerateConstants() or when using the GUI for generating the configuration.
/// </summary>
using System;
using System.Collections.Generic;

namespace ArsemiGlobals {

    public enum Sensors {
		Heartrate,
		Button,
	}


    public class Events {
        public static Dictionary<string, Func<Action?>> EventMap = new(
            [
                			]
		);
	}
}
