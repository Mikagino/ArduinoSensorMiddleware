using Godot;

namespace Components {
    [GlobalClass]
    public partial class State : Node {
        /// <summary>
        /// Executed each call of _Process
        /// </summary>
        public virtual void Update() { }


        /// <summary>
        /// Executed each call of _Process
        /// </summary>
        public virtual void PhysicsUpdate() { }


        /// <summary>
        /// Called when state is entered
        /// </summary>
        public virtual void Enter() { }


        /// <summary>
        /// Called when state is exited
        /// </summary>
        public virtual void Exit() { }
    }
}