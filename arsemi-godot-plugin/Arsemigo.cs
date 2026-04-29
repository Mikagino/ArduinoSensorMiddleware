using Arsemi;
using Godot;

public partial class Arsemigo : Node {
	[Export] public Label HeartrateValueLabel;
	[Export] public Label ButtonValueLabel;
	[Export] public TextureRect Icon;


	private ArsemiCore _arsemiCore = new();


	public override void _Ready() {
		_arsemiCore.StartSetup();
		_arsemiCore.AddSensor(new Arsemi.Sensor.AnalogSensor(), "Heartrate")
			// .AddFilter(new Arsemi.Sensor.Filter.ButterworthFilter(null))
			.SetInterval(250);
		_arsemiCore.AddSensor(new Arsemi.Sensor.DigitalSensor(), "Button");
		// .AddEvent(Arsemi.Sensor.AbstractSensor.EventType.Threshold, "Excitement", 120);
		// _arsemiCore.AddSensor(new Arsemi.Sensor.DigitalSensor(), "Button");
		// _arsemiCore.SetInterval("Button", 200);
		_arsemiCore.ConnectMicrocontroller();
		_arsemiCore.FinishSetup();
		// ExampleConstants.Events.Excitement += EventAction;

		// ArsemiConfig.SaveTo(_asmCore, PathToConfigFile, ArsemiConfig.ConfigType.JSON);
		ConfigSaver.GenerateConstants(_arsemiCore, ProjectSettings.GlobalizePath("res://"));
		_arsemiCore.StartLoop();
	}


	public override void _Process(double delta) {
		// DebugLabel.Text = _asmCore.GetSensorValue((uint)Arsemi.Examples.ExampleConstants.Sensors.Heartrate).ToString();
		//DebugLabel.Text = _arsemiCore.GetSensorValue((uint)ArsemiGlobals.Sensors.Heartrate).ToString();
		//icon.Position = new Vector2(icon.Position.X, _arsemiCore.GetSensorValue((uint)ArsemiGlobals.Sensors.Heartrate));
		var sensorData = _arsemiCore.GetSensorValue((uint)ArsemiGlobals.Sensors.Heartrate);
		HeartrateValueLabel.Text = sensorData.ToString();
		Icon.Position = new Vector2(Icon.Position.X, 400 - sensorData);
		// GD.Print("value: " + sensorData.ToString());
	}
}
