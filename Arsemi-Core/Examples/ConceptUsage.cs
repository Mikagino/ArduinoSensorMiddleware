namespace Arsemi {
    namespace Examples {
        public static class ConceptUsage {
            private static ArsemiCore _asmCore = new();


            public static void Main() {
                Setup();
                while(true) {
                    UpdateLoop();
                }
                Exit();
            }


            public static void Setup() {
                _asmCore.StartSetup();
                _asmCore.AddSensor(new Sensor.AnalogSensor(), "Heartrate")
                    .AddFilter(new Sensor.Filter.BlaBlaFilter())
                    .SetInterval(100);
                _asmCore.AddSensor(new Sensor.DigitalSensor(), "Button");
                _asmCore.SetInterval("Button", 200);
                _asmCore.FinishSetup();
            }


            public static void UpdateLoop() {
                float currentGSR = (float)_asmCore.GetSensorValueID(2);
                float currentHeartrate = (float)_asmCore.GetSensorValue("Heartrate");
            }


            public static void Exit() {
                _asmCore.StopLoop();
            }
        }
    }
}