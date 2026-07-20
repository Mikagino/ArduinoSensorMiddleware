using Godot;
using Weapon;

namespace Player {
    public partial class OutOfScreenIcon : TextureRect {
        [Export] private Vector2 _onScreenOffset = new(0.5f, -5);
        [Export] private float _screenMargin = 4;
        [Export] private float _smoothingSpeed = 8;


        private WeaponItem _weaponItem;
        private Camera2D _camera;


        public void Initialize(WeaponItem weaponItem) {
            _weaponItem = weaponItem;
            _camera = GetViewport().GetCamera2D();
            // Show();
            GD.Print("Init Waypoint ");
        }


        public override void _Process(double delta) {
            if(_weaponItem == null || _camera == null) return;
            Vector2 viewportDimensions = GetViewport().GetVisibleRect().Size;
            Vector2 screenCoordinates = (_weaponItem.GlobalPosition - _camera.GlobalPosition) * _camera.Zoom + viewportDimensions * 0.5f;

            Vector2 clampedScreenCoordinates = new(
                Mathf.Clamp(screenCoordinates.X, _screenMargin, viewportDimensions.X - _screenMargin),
                Mathf.Clamp(screenCoordinates.Y, _screenMargin, viewportDimensions.Y - _screenMargin));

            Vector2 targetDisplayPosition = _camera.GlobalPosition + (clampedScreenCoordinates - viewportDimensions * 0.5f) / _camera.Zoom;
            Vector2 vectorToTarget = _weaponItem.GlobalPosition - targetDisplayPosition;
            float targetDisplayRotation = vectorToTarget.Angle() - Mathf.Pi * 0.5f;
            GD.PrintT(targetDisplayPosition, targetDisplayRotation);

            GlobalPosition = GlobalPosition.Lerp(targetDisplayPosition, (float)delta * _smoothingSpeed);
            Rotation = Mathf.Lerp(Rotation, targetDisplayRotation, (float)delta * _smoothingSpeed);
        }
    }
}