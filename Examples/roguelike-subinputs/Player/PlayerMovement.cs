using Godot;
using Weapon;

namespace Player {
    public partial class PlayerMovement : CharacterBody2D {
        [Export] public int Speed = 400;
        [Export] private WeaponManager _weaponManager;


        private AnimatedSprite2D _sprite;
        private Vector2 _currentDirection;
        private bool _shooting = false;

        public HitboxComponent HitboxComponent;


        public override void _Ready() {
            _sprite = GetNode<AnimatedSprite2D>("%Sprite");
            HitboxComponent = GetNode<HitboxComponent>("%HitboxComponent");
            HitboxComponent.Died += Die;
        }


        private void UpdateInputVector() {
            Velocity = Speed * Input.GetVector(Constants.Inputs.Left, Constants.Inputs.Right, Constants.Inputs.Up, Constants.Inputs.Down);
        }


        /// <summary>
        ///  Rotate the weapon
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta) {
            _weaponManager.LookAt(GetViewport().GetMousePosition());
            if(_shooting) _weaponManager.Shoot(Bullet.BulletSourceType.Player);
        }


        public override void _PhysicsProcess(double delta) {
            UpdateInputVector();
            FlipIcon();
            MoveAndSlide();
        }


        private void FlipIcon() {
            if(_sprite.FlipH && Velocity.X > 0) {
                _sprite.FlipH = false;
            }

            if(!_sprite.FlipH && Velocity.X < 0) {
                _sprite.FlipH = true;
            }
        }


        /// <summary>
        /// Handle inputs (Drop weapon)
        /// </summary>
        /// <param name="event"></param>
        public override void _UnhandledInput(InputEvent @event) {
            if(@event.IsActionPressed(Constants.Inputs.DropItem)) {
                _weaponManager.DropWeapon();
            }
            if(@event.IsActionPressed(Constants.Inputs.Shoot)) {
                _shooting = true;
                // _weaponManager.Shoot(Bullet.BulletSourceType.Player);
            }
            if(@event.IsActionReleased(Constants.Inputs.Shoot)) {
                _shooting = false;
                // _weaponManager.Shoot(Bullet.BulletSourceType.Player);
            }
        }


        private void Die() {
            GD.Print("YOU DIED!!!");
            QueueFree();
        }
    }
}