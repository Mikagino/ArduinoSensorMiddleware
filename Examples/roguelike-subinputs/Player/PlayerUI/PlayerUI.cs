using Godot;

public partial class PlayerUI : Control {
    private ProgressBar _healthbar;


    public override void _Ready() {
        _healthbar = GetNode<ProgressBar>("%HealthBar");
    }


    public void SetHealth(int newHealth) {
        _healthbar.Value = newHealth;
    }
}
