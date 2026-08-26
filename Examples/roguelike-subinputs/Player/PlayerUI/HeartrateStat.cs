using Arsemi.Sensor.Data;
using Godot;
using System;

public partial class HeartrateStat : HBoxContainer {
    [Export] public int MinMaxOffset = 40;
    private ProgressBar _heartrateBar;
    private Label _heartrateLabel;
    private BaselineMeasurement _baseline;

    public override void _Ready() {
        _heartrateBar = GetNode<ProgressBar>("%HeartrateBar");
        _heartrateLabel = GetNode<Label>("%HeartrateLabel");
    }


    public void SetBaseline(BaselineMeasurement baseline) {
        _baseline = baseline;
        _heartrateBar.MinValue = baseline.Min - MinMaxOffset;
        _heartrateBar.MaxValue = baseline.Max + MinMaxOffset;
        _heartrateBar.Value = baseline.Average;
    }


    public void SetHeartrate(int value) {
        _heartrateBar.Value = value;
        _heartrateLabel.Text = value.ToString();
    }
}
