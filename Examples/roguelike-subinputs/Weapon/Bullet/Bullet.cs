using Godot;
using Weapon;

public partial class Bullet : StaticBody2D {
    [Export] private Sprite2D _bulletSprite;
    private WeaponResource _weapon;
    private Vector2 _velocity;

    public void Initialize(WeaponResource weapon, Vector2 velocity) {
        _weapon = weapon;
        _bulletSprite.Texture = weapon.BulletIcon;
        _velocity = velocity;
    }

    public override void _PhysicsProcess(double delta) {
        MoveAndCollide(_velocity * (float)delta * _weapon.BulletSpeed);
    }
}
