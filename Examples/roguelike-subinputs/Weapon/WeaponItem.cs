using Godot;

namespace Weapon {
    public partial class WeaponItem : Sprite2D {
        [Export] public WeaponResource Weapon;


        private Area2D _pickupArea;
        private MeshInstance2D _highlightMesh;


        public override void _Ready() {
            _pickupArea = GetNode<Area2D>("%PickupArea");
            _highlightMesh = GetNode<MeshInstance2D>("%Highlight");
        }

        public void SpawnTo(WeaponResource weapon, Vector2 fromGlobalPosition, Vector2 toGlobalPosition, float durationScale) {
            Disable();
            Weapon = weapon;
            Texture = Weapon.WeaponIcon;
            GlobalPosition = fromGlobalPosition;
            CreateTween()
                .SetEase(Tween.EaseType.Out)
                .TweenProperty(this, "global_position", toGlobalPosition, durationScale * (toGlobalPosition - fromGlobalPosition).Length())
                .Finished += Enable;
        }


        public void Disable() {
            _pickupArea.Monitorable = false;
            _highlightMesh.Hide();
        }


        public void Enable() {
            _pickupArea.Monitorable = true;
            _highlightMesh.Show();
        }
    }
}