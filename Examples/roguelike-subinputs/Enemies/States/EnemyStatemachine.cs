using Components;
using Godot;

namespace Enemies {
    public partial class EnemyStatemachine : Statemachine {
        [Export] private EnemyMovement _enemy;
        [Export] private State _initialState;
        [Export] private State _findNextWeaponState;


        public override void _Ready() {
            TransitionToState(_initialState);
            _enemy.WeaponManager.AmmunitionEmptied += () => TransitionToState(_findNextWeaponState);
        }
    }
}