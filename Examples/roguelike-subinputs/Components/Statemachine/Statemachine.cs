
using Godot;

namespace Components {
    public partial class Statemachine : Node {
        private State CurrentState;


        public override void _Process(double delta) {
            CurrentState.Update();
        }


        public override void _PhysicsProcess(double delta) {
            CurrentState.PhysicsUpdate();
        }


        /// <summary>
        /// Transition to newState and call enter/exit functions on each state
        /// </summary>
        public void TransitionToState(State newState) {
            CurrentState?.Exit();
            newState.Enter();
            CurrentState = newState;
        }
    }
}