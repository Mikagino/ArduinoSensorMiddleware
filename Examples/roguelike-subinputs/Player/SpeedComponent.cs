using Godot;

public partial class SpeedComponent : Timer {
    [ExportGroup("Movement")]
    [Export]
    public int CurrentSpeed {
        set {
            _currentSpeed = value;
            EmitSignal(SignalName.SpeedChanged, _currentSpeed);
        }
        get => _currentSpeed;
    }
    private int _currentSpeed = 400;
    [Export] private int _minSpeed = 200;
    [Export] private int _averageSpeed = 300;
    [ExportGroup("Dodge")]
    [Export] private int _dodgeSpeed = 600;


    [Signal] public delegate void SpeedChangedEventHandler(int speed);


    public override void _Ready() {
        Timeout += () => CurrentSpeed = _averageSpeed;
        CurrentSpeed = _averageSpeed;
    }


    public override void _UnhandledInput(InputEvent @event) {
        if(@event.IsActionPressed(Constants.Inputs.Dodge)) {
            Dodge();
        }
    }


    private void Dodge() {
        CurrentSpeed = _dodgeSpeed;
        Start();
    }
}
