using Godot;

public partial class SampleData : HBoxContainer {
    [Export] private Label valueLabel;
    [Export] private ArsemiGlobals.SensorNames sensor;


    public override void _Process(double delta) {
        Arsemigo.ArsemiCore.GetSensorValue((int)sensor);
    }
}
