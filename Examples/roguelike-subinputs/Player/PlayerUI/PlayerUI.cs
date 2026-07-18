using Godot;

public partial class PlayerUI : Control {
    private ProgressBar _healthbar;
    private Label _ammunitionLabel;


    public override void _Ready() {
        _healthbar = GetNode<ProgressBar>("%HealthBar");
        _ammunitionLabel = GetNode<Label>("%AmmunitionLabel");
    }


    public void SetHealth(int newHealth) {
        _healthbar.Value = newHealth;
    }


    public void SetAmmunition(int newAmmunition) {
        _ammunitionLabel.Text = newAmmunition.ToString();
    }
}
