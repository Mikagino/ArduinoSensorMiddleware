using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Player;
using Weapon;


namespace Enemies {
    public partial class EnemyMovement : CharacterBody2D {
        [Export] public float MovementSpeed = 300;
        [Export] public float WeaponSearchChunkStepSize = 200;
        [Export] public float WeaponSearchChunkMaximum = 600;
        private PlayerMovement _player;

        public HitboxComponent HitboxComponent;
        private WeaponManager _weaponManager;
        private Timer _shootTimer;
        private NavigationAgent2D _navigationAgent;
        private Area2D _weaponSearchChunk;


        public override void _Ready() {
            _player = GetTree().GetNodesInGroup(Constants.Groups.Player)[0] as PlayerMovement;
            HitboxComponent = GetNode<HitboxComponent>("%HitboxComponent");
            _navigationAgent = GetNode<NavigationAgent2D>("%NavigationAgent2D");
            _shootTimer = GetNode<Timer>("%ShootTimer");
            _weaponManager = GetNode<WeaponManager>("%WeaponManager");
            _weaponSearchChunk = GetNode<Area2D>("%WeaponSearchChunk");

            HitboxComponent.Died += Die;

            _shootTimer.Timeout += () => _weaponManager.Shoot();
            _weaponManager.WeaponChanged += SetWeaponTimer;
            _weaponManager.AmmunitionEmptied += WalkToNextWeapon;

            _navigationAgent.VelocityComputed += UpdateMoveAndSlide;
        }


        public override void _Process(double delta) {
            if(_player == null) return;
            _weaponManager.LookAt(_player.GlobalPosition);
        }


        private void SetWeaponTimer(WeaponResource weapon) {
            _shootTimer.WaitTime = weapon.AttackSpeed;
        }


        public void Die() {
            QueueFree();
        }


        #region Movement
        private void SetMovementTarget(Vector2 movementTarget) {
            _navigationAgent.TargetPosition = movementTarget;
        }


        /// <summary>
        /// Movement based on: https://docs.godotengine.org/en/latest/tutorials/navigation/navigation_using_navigationagents.html
        /// </summary>
        public override void _PhysicsProcess(double delta) {
            // Do not query when the map has never synchronized and is empty.
            if(NavigationServer2D.MapGetIterationId(_navigationAgent.GetNavigationMap()) == 0) {
                return;
            }

            if(_navigationAgent.IsNavigationFinished()) {
                return;
            }

            Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
            Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * MovementSpeed;
            if(_navigationAgent.AvoidanceEnabled) {
                _navigationAgent.Velocity = newVelocity;
            }
            else {
                UpdateMoveAndSlide(newVelocity);
            }
        }


        private void UpdateMoveAndSlide(Vector2 safeVelocity) {
            Velocity = safeVelocity;
            MoveAndSlide();
        }


        public void WalkToNextWeapon() {
            WalkToNextWeaponAsync();
        }


        public async Task WalkToNextWeaponAsync() {
            if(_weaponManager.CurrentWeapon != null || !_navigationAgent.IsNavigationFinished()) return;
            var weaponItem = GetFirstWeaponInsideSearchChunk();
            for(var i = 0; (i + 1) * WeaponSearchChunkStepSize <= WeaponSearchChunkMaximum && weaponItem == null; i++) {
                (_weaponSearchChunk.GetChild<CollisionShape2D>(0).Shape as CircleShape2D).Radius += WeaponSearchChunkStepSize;
                await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
                weaponItem = GetFirstWeaponInsideSearchChunk();
            }
            if(weaponItem == null) throw new Exception("No weapon could be found!");
            SetMovementTarget(weaponItem.GlobalPosition);
            if(!_navigationAgent.IsNavigationFinished())
                _navigationAgent.Connect(NavigationAgent2D.SignalName.NavigationFinished, Callable.From(WalkToNextWeapon), (uint)ConnectFlags.OneShot);
        }


        private WeaponItem? GetFirstWeaponInsideSearchChunk() {
            var weapons = _weaponSearchChunk.GetOverlappingAreas();
            GD.Print("Chunk found: " + (weapons.Count != 0 ? "weapon" : "nothing"));
            return weapons.Count != 0 ? weapons.First().GetParent<WeaponItem>() : null;
        }
        #endregion Movement
    }
}