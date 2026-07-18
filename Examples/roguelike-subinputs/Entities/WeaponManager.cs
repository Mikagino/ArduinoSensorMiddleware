using Godot;

namespace Weapon {
    public partial class WeaponManager : Sprite2D {
        [Export] public WeaponResource? CurrentWeapon;
        [Export] private PackedScene _weaponItemScene;
        [Export] private PackedScene _bulletScene;
        [Export]
        public int CurrentAmmunition {
            private set {
                _currentAmmunition = value;
                EmitSignal(SignalName.AmmunitionChanged, CurrentAmmunition);
            }
            get => _currentAmmunition;
        }
        private int _currentAmmunition;


        private Node _droppedWeaponsContainer;
        private Node _bulletContainer;
        private long _lastShot = 0;


        [Signal] public delegate void AmmunitionChangedEventHandler(int newAmmunition);
        [Signal] public delegate void AmmunitionEmptiedEventHandler();


        public override void _Ready() {
            _droppedWeaponsContainer = GetNode<Node>("%DroppedWeapons");
            _bulletContainer = GetNode<Node>("%BulletContainer");
            PickupWeapon(CurrentWeapon);
        }


        public void DropWeapon() {
            EmitSignal(SignalName.AmmunitionEmptied);
            WeaponItem weaponItemInstance = _weaponItemScene.Instantiate<WeaponItem>();
            weaponItemInstance.Initialize(CurrentWeapon, GlobalPosition);
            _droppedWeaponsContainer.AddChild(weaponItemInstance);
            CurrentWeapon = null;
            Texture = null;
        }


        public void PickupWeapon(WeaponResource weapon) {
            CurrentWeapon = weapon;
            Texture = CurrentWeapon.WeaponIcon;
            CurrentAmmunition = CurrentWeapon.Ammunition;
        }


        public void Shoot(Bullet.BulletSourceType? bulletSourceType = null) {
            if(CurrentAmmunition <= 0) return;
            if(_lastShot + CurrentWeapon.AttackSpeed > (long)Time.GetTicksMsec()) return;

            Bullet newBullet = _bulletScene.Instantiate<Bullet>();
            newBullet.GlobalPosition = GlobalPosition + (Offset * 1.5f).Rotated(Rotation);
            newBullet.Initialize(CurrentWeapon, Vector2.Right.Rotated(Rotation), bulletSourceType);
            _bulletContainer.AddChild(newBullet);
            _lastShot = (long)Time.GetTicksMsec();
            CurrentAmmunition--;
        }


        /// <summary>
        /// Yeet weapon away when ammunition falls below 0
        /// </summary>
        // public void YEET() {
        //     WeaponItem yeetedWeapon = _weaponItemScene.Instantiate<WeaponItem>();

        // }
    }
}