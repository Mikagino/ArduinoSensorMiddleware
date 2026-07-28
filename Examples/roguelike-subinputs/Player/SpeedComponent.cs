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
    [Export] private int _maximumStamina = 5;
    [Export]
    public int CurrentStamina {
        set {
            _currentStamina = Mathf.Clamp(value, 0, _maximumStamina);
            EmitSignal(SignalName.StaminaChanged, _currentStamina);
        }
        get => _currentStamina;
    }
    private int _currentStamina;


    [Signal] public delegate void SpeedChangedEventHandler(int speed);
    [Signal] public delegate void DodgeStartedEventHandler();
    [Signal] public delegate void DodgeEndedEventHandler();
    [Signal] public delegate void StaminaChangedEventHandler(int value);
    [Signal] public delegate void StaminaSetupEventHandler(int maximumStamina);


    private Timer _staminaRegenerationTimer;


    public override void _Ready() {
        Timeout += EndDodge;
        CurrentSpeed = _averageSpeed;

        EmitSignal(SignalName.StaminaSetup, _maximumStamina);
        CurrentStamina = _maximumStamina;
        _staminaRegenerationTimer = GetNode<Timer>("%StaminaRegenerateTimer");
        _staminaRegenerationTimer.Timeout += RegenerateStamina;

    }


    public override void _UnhandledInput(InputEvent @event) {
        if(@event.IsActionPressed(Constants.Inputs.Dodge)) {
            StartDodge();
        }
    }


    private void StartDodge() {
        if(CurrentStamina <= 0) return;
        CurrentSpeed = _dodgeSpeed;
        Start();
        EmitSignal(SignalName.DodgeStarted);
        CurrentStamina--;
    }


    private void EndDodge() {
        CurrentSpeed = _averageSpeed;
        EmitSignal(SignalName.DodgeEnded);
    }


    private void RegenerateStamina() {
        CurrentStamina = Mathf.Clamp(CurrentStamina + 1, 0, _maximumStamina);
    }
}
