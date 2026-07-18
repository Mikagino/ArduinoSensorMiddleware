using Godot;

namespace Weapon {
    [Tool]
    public partial class WeaponItem : Sprite2D {
        [Export] public WeaponResource Weapon;


        public void Initialize(WeaponResource weapon, Vector2 globalPosition) {
            Weapon = weapon;
            Texture = Weapon.WeaponIcon;
            GlobalPosition = globalPosition;
        }
    }
}