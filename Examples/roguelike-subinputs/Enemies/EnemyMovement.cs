using Components;
using Godot;
using Player;

namespace Enemies {
    public partial class EnemyMovement : CharacterBody2D {
        [Export] public float MovementSpeed = 200;


        [ExportGroup("Components")]
        [Export] public HitboxComponent HitboxComponent;
        [Export] public WeaponManager WeaponManager;
        [Export] public NavigationAgent2D NavigationAgent;


        private PlayerMovement _player;


        public override void _Ready() {
            _player = GetTree().GetNodesInGroup(Constants.Groups.Player)[0] as PlayerMovement;
            NavigationAgent.VelocityComputed += UpdateMoveAndSlide;
        }


        public void Die() {
            WeaponManager.DropWeapon();
            QueueFree();
        }


        #region Movement
        public void SetMovementTarget(Vector2 movementTarget) {
            NavigationAgent.TargetPosition = movementTarget;
        }


        /// <summary>
        /// Movement based on: https://docs.godotengine.org/en/latest/tutorials/navigation/navigation_using_navigationagents.html
        /// </summary>
        public override void _PhysicsProcess(double delta) {
            // Do not query when the map has never synchronized and is empty.
            if(NavigationServer2D.MapGetIterationId(NavigationAgent.GetNavigationMap()) == 0) {
                return;
            }

            if(NavigationAgent.IsNavigationFinished()) {
                return;
            }

            Vector2 nextPathPosition = NavigationAgent.GetNextPathPosition();
            Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * MovementSpeed;
            if(NavigationAgent.AvoidanceEnabled) {
                NavigationAgent.Velocity = newVelocity;
            }
            else {
                UpdateMoveAndSlide(newVelocity);
            }
        }


        private void UpdateMoveAndSlide(Vector2 safeVelocity) {
            Velocity = safeVelocity;
            MoveAndSlide();
        }
        #endregion Movement
    }
}