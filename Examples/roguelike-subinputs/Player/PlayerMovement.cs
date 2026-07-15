using Godot;

namespace Player {
    public partial class PlayerMovement : CharacterBody2D {
        [Export] public int Speed = 400;


        private AnimatedSprite2D _sprite;
        private Vector2 _currentDirection;


        public override void _Ready() {
            _sprite = GetNode<AnimatedSprite2D>("%Sprite");
        }


        private void UpdateInputVector() {
            Velocity = Speed * Input.GetVector(Constants.Inputs.Left, Constants.Inputs.Right, Constants.Inputs.Up, Constants.Inputs.Down);
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
    }
}