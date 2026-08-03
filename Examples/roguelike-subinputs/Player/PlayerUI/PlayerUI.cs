using Godot;

namespace Player {
    public partial class PlayerUI : Control {
        private ProgressBar _healthbar;
        private Label _ammunitionLabel;
        private Control _gameOver;
        private OutOfScreenIcon _outOfScreenIcon;
        [Export] private StaminaBar _staminaBar;


        public override void _Ready() {
            _healthbar = GetNode<ProgressBar>("%HealthBar");
            _ammunitionLabel = GetNode<Label>("%AmmunitionLabel");
            _gameOver = GetNode<Control>("%GameOver");
            _outOfScreenIcon = GetNode<OutOfScreenIcon>("%OutOfScreenIcon");
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


        public void SetStamina(int value) {
            _staminaBar.SetStamina(value);
        }


        public void InitializeStamina(int maximumStamina) {
            _staminaBar.InitializeStamina(maximumStamina);
        }


        // public void SetWeaponOutOfScreen(Area2D weapon) {
        //     if(weapon.GetParent() is not WeaponItem) {
        //         GD.Print("Out of screen not weapon");
        //         return;
        //     }

        //     _outOfScreenIcon.Initialize(weapon.GetParent<WeaponItem>());
        // }


        public void GameOver() {
            GetTree().Paused = true;
            _gameOver.Show();
        }


        public void ReloadCurrentScene() {
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        }


        public void ExitGame() {
            GetTree().Quit();
        }
    }
}