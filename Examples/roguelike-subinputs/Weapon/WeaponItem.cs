using System.Threading.Tasks;
using Godot;

namespace Weapon {
    public partial class WeaponItem : RigidBody2D {
        [Export] public WeaponResource Weapon;


        [ExportGroup("Components")]
        [Export] private CollisionShape2D _pickupCollision;
        [Export] private Sprite2D _sprite;
        [Export] private MeshInstance2D _highlightMesh;


        public bool IsVisibleOnScreen = true;

        private int _initialEnablingDelay = 500;


        public override void _Ready() {
            _sprite.Texture = Weapon.WeaponIcon;
        }


        public async Task SpawnTo(WeaponResource weapon, Vector2 fromGlobalPosition, Vector2 impulse) {
            Disable();
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
            Weapon = weapon;
            _sprite.Texture = Weapon.WeaponIcon;
            GlobalPosition = fromGlobalPosition;

            ApplyImpulse(impulse);
            await Task.Delay(500);
            Enable();
        }


        public void Disable() {
            _highlightMesh.Hide();
            _pickupCollision.Disabled = true;
        }


        public void Enable() {
            _highlightMesh.Show();
            _pickupCollision.Disabled = false;
        }


        public void SetIsVisibleOnScreen(bool isVisible) {
            IsVisibleOnScreen = isVisible;
        }
    }
}