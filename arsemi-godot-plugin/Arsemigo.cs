using Arsemi;
using Arsemi.Sensor;
using Godot;

public partial class Arsemigo : Node {
	[Export] public Label HeartrateValueLabel;
	[Export] public Label ButtonValueLabel;
	[Export] public TextureRect Icon;


	public static ArsemiCore ArsemiCore = new();


	public override void _Ready() {
		ArsemiCore.StartSetup();
		ArsemiCore.AddSensor(new MAX30102Sensor("Heartrate"))
			// .AddFilter(new Arsemi.Sensor.Filter.ButterworthFilter(null))
			.SetInterval(100);
		ArsemiCore.AddSensor(new DigitalSensor("Button", 2));
		// .AddEvent(Arsemi.Sensor.AbstractSensor.EventType.Threshold, "Excitement", 120);
		// _arsemiCore.AddSensor(new Arsemi.Sensor.DigitalSensor(), "Button");
		// _arsemiCore.SetInterval("Button", 200);
		ArsemiCore.ConnectMicrocontroller();
		ArsemiCore.FinishSetup();
		// ExampleConstants.Events.Excitement += EventAction;

		// ArsemiConfig.SaveTo(_asmCore, PathToConfigFile, ArsemiConfig.ConfigType.JSON);
		ConfigSaver.GenerateConstants(ArsemiCore, ProjectSettings.GlobalizePath("res://"));
		ArsemiCore.StartLoop();
	}


	public override void _Process(double delta) {
		// DebugLabel.Text = _asmCore.GetSensorValue((uint)Arsemi.Examples.ExampleConstants.Sensors.Heartrate).ToString();
		//DebugLabel.Text = _arsemiCore.GetSensorValue((uint)ArsemiGlobals.Sensors.Heartrate).ToString();
		//icon.Position = new Vector2(icon.Position.X, _arsemiCore.GetSensorValue((uint)ArsemiGlobals.Sensors.Heartrate));

		// var sensorData = ArsemiCore.GetSensorValue((uint)ArsemiGlobals.SensorNames.Heartrate);
		// HeartrateValueLabel.Text = sensorData.ToString();
		// Icon.Position = new Vector2(Icon.Position.X, 400 - sensorData);

		// GD.Print("value: " + sensorData.ToString());
	}
}
