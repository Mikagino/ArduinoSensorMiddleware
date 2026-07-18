using Godot;

namespace Weapon {
    public partial class WeaponManager : Sprite2D {
        [Export] public WeaponResource CurrentWeapon;
        [Export] private PackedScene _weaponItemScene;
        [Export] private PackedScene _bulletScene;


        private Node _droppedWeaponsContainer;
        private Node _bulletContainer;
        private long _lastShot = 0;


        public override void _Ready() {
            _droppedWeaponsContainer = GetNode<Node>("%DroppedWeapons");
            _bulletContainer = GetNode<Node>("%BulletContainer");
            PickupWeapon(CurrentWeapon);
        }


        public void DropWeapon() {
            Sprite2D weaponItemInstance = _weaponItemScene.Instantiate<Sprite2D>();
            _droppedWeaponsContainer.AddChild(weaponItemInstance);
        }


        public void PickupWeapon(WeaponResource weapon) {
            CurrentWeapon = weapon;
            Texture = CurrentWeapon.WeaponIcon;
        }


        public void Shoot(Bullet.BulletSourceType? bulletSourceType = null) {
            if(_lastShot + CurrentWeapon.AttackSpeed > (long)Time.GetTicksMsec()) return;

            Bullet newBullet = _bulletScene.Instantiate<Bullet>();
            newBullet.GlobalPosition = GlobalPosition + (Offset * 1.5f).Rotated(Rotation);
            newBullet.Initialize(CurrentWeapon, Vector2.Right.Rotated(Rotation), bulletSourceType);
            _bulletContainer.AddChild(newBullet);
            _lastShot = (long)Time.GetTicksMsec();
        }
    }
}