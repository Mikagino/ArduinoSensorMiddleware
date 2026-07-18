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
            HitboxComponent.Died += Die;
        }


        public override void _Process(double delta) {
            if(_player == null) return;
            _weaponManager.LookAt(_player.GlobalPosition);
            _weaponManager.Shoot();
        }


        public void Die() {
            QueueFree();
        }
    }
}