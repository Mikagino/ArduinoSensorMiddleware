using Godot;

namespace Weapon {
    [Tool]
    public partial class WeaponItem : Sprite2D {
        [Export] public WeaponResource Weapon;


        public void SpawnTo(WeaponResource weapon, Vector2 fromGlobalPosition, Vector2 toGlobalPosition, float durationScale) {
            Weapon = weapon;
            Texture = Weapon.WeaponIcon;
            GlobalPosition = fromGlobalPosition;
            CreateTween()
                .SetEase(Tween.EaseType.Out)
                .TweenProperty(this, "global_position", toGlobalPosition, durationScale * (toGlobalPosition - fromGlobalPosition).Length());
        }
    }
}