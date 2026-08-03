using Arsemi.Sensor;
using Godot;

public partial class ArsemiComponent : Node {
    [Signal] public delegate void HeartrushEventHandler();
    [Signal] public delegate void SweatlessEventHandler();
    [Signal] public delegate void NewDataReceivedEventHandler(int sensorId, int value);

    private const string HeartrushEvent = "Heartrush";
    private const string SweatlessEvent = "SweatlessEvent";


    public override void _Ready() {
        Arsemigo.Instance.AddSensor(new MAX30102Sensor("HeartrateSensor"))
            .SetInterval(100)
            .AddEvent(HeartrushEvent, rb => Arsemi.Sensor.Event.EventCondition.AboveThreshold(rb, 70));

        Arsemigo.Instance.AddSensor(new AnalogSensor("GSR-Sensor", 0))
            .SetInterval(255)
            .AddEvent(SweatlessEvent, rb => Arsemi.Sensor.Event.EventCondition.AboveThreshold(rb, 30));

        Arsemigo.Instance.AddSensor(new AnalogSensor("EMG-Sensor", 1))
            .SetInterval(30);

        Arsemigo.Instance.AddSensor(new DigitalSensor("Button-Sensor", 0));


        Arsemigo.Instance.EventReceived += HandleEvents;
        Arsemigo.Instance.NewDataReceived += (a, b) => EmitSignal(SignalName.NewDataReceived, a, b);
    }


    private void HandleEvents(Arsemi.Sensor.Event.EventData eventData) {
        switch(eventData.Name) {
        case HeartrushEvent:
            EmitSignal(SignalName.Heartrush);
            break;
        case SweatlessEvent:
            EmitSignal(SignalName.Sweatless);
            break;
        }
    }
}
