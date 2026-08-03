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
			.SetInterval(200);
		ArsemiCore.AddSensor(new DigitalSensor("Button", 2))
			.SetInterval(200);
		ArsemiCore.ConnectMicrocontroller();
		ArsemiCore.FinishSetup();

		// ArsemiConfig.SaveTo(_asmCore, PathToConfigFile, ArsemiConfig.ConfigType.JSON);
		ConfigSaver.GenerateGlobals(ArsemiCore, ProjectSettings.GlobalizePath("res://"));
		ArsemiCore.StartLoop();
	}
}
