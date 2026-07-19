using System;
using Godot;

public partial class PlayerUI : Control {
    private ProgressBar _healthbar;
    private Label _ammunitionLabel;
    private Control _gameOver;


    public override void _Ready() {
        _healthbar = GetNode<ProgressBar>("%HealthBar");
        _ammunitionLabel = GetNode<Label>("%AmmunitionLabel");
        _gameOver = GetNode<Control>("%GameOver");
    }


    public void SetHealth(int newHealth) {
        _healthbar.Value = newHealth;
    }


    public void SetAmmunition(int newAmmunition) {
        if(_ammunitionLabel == null) return;
        _ammunitionLabel.Text = newAmmunition.ToString();
        _ammunitionLabel.SelfModulate = Color.Color8(255, 255, 255, 255);
    }


    public void AmmunitionEmptied() {
        _ammunitionLabel.Text = "X";
        _ammunitionLabel.SelfModulate = Color.Color8(255, 0, 0, 255);
    }


    public void GameOver() {
        GetTree().Paused = true;
        _gameOver.Show();
    }


    public void ReloadCurrentScene() {
        GetTree().ReloadCurrentScene();
        GetTree().Paused = false;
    }


    public void ExitGame() {
        GetTree().Quit();
    }
}
