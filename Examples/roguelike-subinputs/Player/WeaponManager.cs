using Godot;

namespace Weapon {
    public partial class WeaponManager : Sprite2D {
        [Export] public WeaponResource CurrentWeapon;
        [Export] private PackedScene _weaponItemScene;
        [Export] private PackedScene _bulletScene;


        private Node _droppedWeaponsContainer;
        private Node _bulletContainer;


        public override void _Ready() {
            _droppedWeaponsContainer = GetNode<Node>("%DroppedWeapons");
            _bulletContainer = GetNode<Node>("%BulletContainer");
            PickupWeapon(CurrentWeapon);
        }


        /// <summary>
        /// Handle inputs (Drop weapon)
        /// </summary>
        /// <param name="event"></param>
        public override void _UnhandledInput(InputEvent @event) {
            if(@event.IsActionPressed(Player.Constants.Inputs.DropItem)) {
                DropWeapon();
            }
            if(@event.IsActionPressed(Player.Constants.Inputs.Shoot)) {
                Shoot();
            }
        }


        public void DropWeapon() {
            Sprite2D weaponItemInstance = _weaponItemScene.Instantiate<Sprite2D>();
            _droppedWeaponsContainer.AddChild(weaponItemInstance);
        }


        public void PickupWeapon(WeaponResource weapon) {
            CurrentWeapon = weapon;
            Texture = CurrentWeapon.WeaponIcon;
        }


        public void Shoot() {
            Bullet newBullet =_bulletScene.Instantiate<Bullet>();
            newBullet.GlobalPosition = GlobalPosition + Offset.Rotated(Rotation);
            newBullet.Initialize(CurrentWeapon, Vector2.Right.Rotated(Rotation));
            _bulletContainer.AddChild(newBullet);
        }


        /// <summary>
        /// Handles weapon rotation
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta) {
            LookAt(GetViewport().GetMousePosition());
        }
    }
}