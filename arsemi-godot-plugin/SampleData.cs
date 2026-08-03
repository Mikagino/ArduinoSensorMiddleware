using Godot;

public partial class SampleData : HBoxContainer {
    [Export] private Label valueLabel;
    [Export] private ArsemiGlobals.SensorNames sensor;


    public override void _Process(double delta) {
        // GD.Print(Arsemigo.ArsemiCore.GetSensorValue((int)sensor));
        Arsemigo.ArsemiCore.NewDataReceived += UpdateValueLabel;
    }

    public void UpdateValueLabel(int sensorId, int value) {
        if(sensorId == (int)sensor) {
            valueLabel.Text = value.ToString();
            GD.Print("Sensor: " + sensorId + " | new value: " + value);
        }
    }
}
