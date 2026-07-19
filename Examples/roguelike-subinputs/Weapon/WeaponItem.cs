using Godot;

namespace Weapon {
    public partial class WeaponItem : Sprite2D {
        [Export] public WeaponResource Weapon;


        [Export] private Area2D _pickupArea;


        public void SpawnTo(WeaponResource weapon, Vector2 fromGlobalPosition, Vector2 toGlobalPosition, float durationScale) {
            _pickupArea.Monitorable = false;
            Weapon = weapon;
            Texture = Weapon.WeaponIcon;
            GlobalPosition = fromGlobalPosition;
            CreateTween()
                .SetEase(Tween.EaseType.Out)
                .TweenProperty(this, "global_position", toGlobalPosition, durationScale * (toGlobalPosition - fromGlobalPosition).Length())
                .Finished += () => { _pickupArea.Monitorable = true; };
        }
    }
}