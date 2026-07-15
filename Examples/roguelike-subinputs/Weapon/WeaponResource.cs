using Godot;

namespace Weapon {
    [GlobalClass]
    public partial class WeaponResource : Resource {
        [Export] public float AttackSpeed = 1;
        [Export] public int Ammunition = 10;
        [Export] public int Damage = 1;
        [Export] public int BulletSpeed = 400;
        [Export] public Texture2D WeaponIcon;
        [Export] public Texture2D BulletIcon;
    }
}