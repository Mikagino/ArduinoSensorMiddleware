using Godot;

namespace Weapon {
    public partial class WeaponItem : StaticBody2D {
        [Export] public WeaponResource Weapon;


        public bool IsVisibleOnScreen = true;


        [ExportGroup("Components")]
        [Export] private CollisionShape2D _pickupArea;
        [Export] private Sprite2D _sprite;
        [Export] private MeshInstance2D _highlightMesh;


        public void SpawnTo(WeaponResource weapon, Vector2 fromGlobalPosition, Vector2 toGlobalPosition, float durationScale) {
            Disable();
            Weapon = weapon;
            _sprite.Texture = Weapon.WeaponIcon;
            GlobalPosition = fromGlobalPosition;
            CreateTween()
                .SetEase(Tween.EaseType.Out)
                .TweenProperty(this, "global_position", toGlobalPosition, durationScale * (toGlobalPosition - fromGlobalPosition).Length())
                .Finished += Enable;
        }


        public void Disable() {
            _pickupArea.Disabled = true;
            _highlightMesh.Hide();
        }


        public void Enable() {
            _pickupArea.Disabled = false;
            _highlightMesh.Show();
        }


        public void SetIsVisibleOnScreen(bool isVisible) {
            IsVisibleOnScreen = isVisible;
        }
    }
}