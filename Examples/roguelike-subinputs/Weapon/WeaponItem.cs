using Godot;

namespace Weapon {
    [Tool]
    public partial class WeaponItem : Sprite2D {
        [Export]
        public WeaponResource Weapon;
    }
}