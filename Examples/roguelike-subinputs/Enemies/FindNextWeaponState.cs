using Enemies;
using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using Weapon;

namespace Components {
    /// <summary>
    /// Random movement based on player position, will move around the player and sometimes closer/away
    /// </summary>
    public partial class FindNextWeaponState : State {
        [Export] public float WeaponSearchChunkStepSize = 200;
        [Export] public float WeaponSearchChunkMaximum = 600;
        [Export] public int InitialSearchDelayMillis = 500;
        [Export] public int SearchMovementSpeed = 200;


        [ExportGroup("Components")]
        [Export] private EnemyMovement _enemy;
        [Export] private Area2D _weaponSearchChunk;
        [Export] private Statemachine _statemachine;
        [Export] private State _randomWalkShootState;


        private WeaponItem? _nextWeapon = null;

        private bool _searching = false;


        public override void Enter() {
            _enemy.MovementSpeed = SearchMovementSpeed;
        }


        public override void Update() {
            if(!_searching && (_nextWeapon == null || !IsInstanceValid(_nextWeapon))) {
                WalkToNextWeaponAsync();
            }

            if(_nextWeapon != null && IsInstanceValid(_nextWeapon)) {
                _enemy.SetMovementTarget(_nextWeapon.GlobalPosition);
            }

            if(_enemy.WeaponManager.CurrentWeapon != null) {
                _nextWeapon = null;
                _statemachine.TransitionToState(_randomWalkShootState);
            }
        }


        public async Task WalkToNextWeaponAsync() {
            _searching = true;
            await Task.Delay(InitialSearchDelayMillis);
            _nextWeapon = GetFirstWeaponInsideSearchChunk();
            CircleShape2D circleCollisionShape = _weaponSearchChunk.GetChild<CollisionShape2D>(0).Shape as CircleShape2D;
            for(int i = 0; (i + 1) * WeaponSearchChunkStepSize <= WeaponSearchChunkMaximum && _nextWeapon == null; i++) {
                circleCollisionShape.Radius += WeaponSearchChunkStepSize;
                await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
                _nextWeapon = GetFirstWeaponInsideSearchChunk();
            }
            if(_nextWeapon == null) throw new Exception("No weapon could be found!");
            circleCollisionShape.Radius = WeaponSearchChunkStepSize;
            _searching = false;
        }


        private WeaponItem? GetFirstWeaponInsideSearchChunk() {
            Godot.Collections.Array<Area2D> weapons = _weaponSearchChunk.GetOverlappingAreas();
            return weapons.Count != 0 ? weapons.First().GetParent<WeaponItem>() : null;
        }
    }
}