using System.Threading;
using Arsemi;
using Godot;

public partial class Arsemigo : Node {
    [Export] public Label DebugLabel;

    private ArsemiCore _asmCore = new();

    public override void _Ready() {
        _asmCore.StartSetup();
        _asmCore.AddSensor(new Arsemi.Sensor.AnalogSensor(), "Heartrate")
            .AddFilter(new Arsemi.Sensor.Filter.BlaBlaFilter())
            .SetInterval(100)
            .AddEvent(Arsemi.Sensor.AbstractSensor.EventType.Threshold, "Excitement", 120);
        _asmCore.AddSensor(new Arsemi.Sensor.DigitalSensor(), "Button");
        _asmCore.SetInterval("Button", 200);
        _asmCore.FinishSetup();
        // ExampleConstants.Events.Excitement += EventAction;

        // ArsemiConfig.SaveTo(_asmCore, PathToConfigFile, ArsemiConfig.ConfigType.JSON);
        _asmCore.StartLoop();
    }

    public override void _Process(double delta) {
        DebugLabel.Text = _asmCore.GetSensorValue((uint)Arsemi.Examples.ExampleConstants.Sensors.Heartrate).ToString();
    }
}
