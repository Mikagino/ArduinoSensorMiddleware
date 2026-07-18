using System;
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


        [ExportGroup("Yeet")]
        [Export] public int MinYeetRange = 50;
        [Export] public int MaxYeetRange = 300;
        [Export] public float YeetDurationScale = 0.001f;


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


        /// <summary>
        /// Yeets the weapon in a random direction with a random distance
        /// </summary>
        public void DropWeapon() {
            float randomLength = Random.Shared.Next(MinYeetRange, MaxYeetRange);
            float randomRotation = Random.Shared.Next(63) / 10;
            Vector2 yeetOffset = Vector2.Up.Rotated(randomRotation) * randomLength;

            WeaponItem weaponItemInstance = _weaponItemScene.Instantiate<WeaponItem>();
            weaponItemInstance.SpawnTo(CurrentWeapon, GlobalPosition, GlobalPosition + yeetOffset, YeetDurationScale);
            weaponItemInstance.Rotate(randomRotation);
            _droppedWeaponsContainer.AddChild(weaponItemInstance);

            CurrentWeapon = null;
            Texture = null;
            EmitSignal(SignalName.AmmunitionEmptied);
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
    }
}