namespace ArSeMi {
    namespace Examples {
        public class ConceptUsage {
            private ArsemiCore _asmCore = new();

            public void Setup() {
                _asmCore.StartSetup();
                _asmCore.AddSensor(new Sensor.AnalogSensor(), "Heartrate")
                    .AddFilter(new Sensor.Filter.BlaBlaFilter())
                    .SetInterval(100);
                _asmCore.AddSensor(new Sensor.DigitalSensor(), "Button");
                _asmCore.SetInterval("Button", 200);
                _asmCore.FinishSetup();
            }


            public void UpdateLoop() {
                float currentGSR = (float)_asmCore.GetSensorValueID(2);
                float currentHeartrate = (float)_asmCore.GetSensorValue("Heartrate");
            }


            public void Exit() {
                _asmCore.StopLoop();
            }
        }
    }
}