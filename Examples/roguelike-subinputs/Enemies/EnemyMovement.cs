using Godot;
using Player;
using Weapon;


namespace Enemies {
    public partial class EnemyMovement : CharacterBody2D {
        [Export] private WeaponManager _weaponManager;
        [Export] private Timer _shootTimer;
        private PlayerMovement _player;

        public HitboxComponent HitboxComponent;


        public override void _Ready() {
            HitboxComponent = GetNode<HitboxComponent>("%HitboxComponent");
            _player = GetTree().GetNodesInGroup(Constants.Groups.Player)[0] as PlayerMovement;
            _shootTimer.WaitTime = _weaponManager.CurrentWeapon.AttackSpeed;
            _shootTimer.Timeout += () => _weaponManager.Shoot();
        }


        public override void _Process(double delta) {
            _weaponManager.LookAt(_player.GlobalPosition);
        }
    }
}