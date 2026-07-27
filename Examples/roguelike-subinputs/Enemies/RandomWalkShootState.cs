using Enemies;
using Godot;
using Player;
using System;
using Weapon;

namespace Components {
    /// <summary>
    /// Random movement based on player position, will move around the player and sometimes closer/away
    /// </summary>
    public partial class RandomWalkShootState : State {
        [ExportGroup("Randomized Movement")]
        [Export] public int MinRotationOffset = 20;
        [Export] public int MinMoveDistance = 100;
        [Export] public int MaxMoveDistance = 500;
        [Export] public int ShootMovementSpeed = 100;


        [ExportGroup("Components")]
        [Export] private EnemyMovement _enemy;
        [Export] private NavigationAgent2D _navigationAgent;
        [Export] private WeaponManager _weaponManager;
        [Export] private Timer _shootTimer;


        private PlayerMovement _player;
        private float _quarterRotation = Mathf.Pi / 2f;


        public override void _Ready() {
            _player = GetTree().GetNodesInGroup(Constants.Groups.Player)[0] as PlayerMovement;
            _weaponManager.WeaponChanged += SetWeaponTimer;

            _shootTimer.Timeout += () => _weaponManager.Shoot();
            if(_weaponManager.CurrentWeapon != null) {
                _shootTimer.WaitTime = _weaponManager.CurrentWeapon.AttackSpeed / 1000;
            }
        }


        public override void Enter() {
            _enemy.MovementSpeed = ShootMovementSpeed;
        }


        private void SetWeaponTimer(WeaponResource weapon) {
            _shootTimer.WaitTime = weapon.AttackSpeed / 1000;
        }


        public override void Update() {
            if(_player == null) return;
            _weaponManager.LookAt(_player.GlobalPosition);

            if(!_navigationAgent.IsNavigationFinished()) return;
            Vector2 vectorToPlayer = (_player.GlobalPosition - _enemy.GlobalPosition).Normalized();
            float randomMovementRotationOffset = Mathf.DegToRad(Random.Shared.Next(MinRotationOffset));
            float randomDirection = ((Random.Shared.Next() % 2 == 0) ? _quarterRotation : -_quarterRotation) + randomMovementRotationOffset;
            Vector2 targetPosition = _enemy.GlobalPosition + (vectorToPlayer.Rotated(randomDirection) * Random.Shared.Next(MinMoveDistance, MaxMoveDistance));
            _enemy.SetMovementTarget(targetPosition);
        }
    }
}