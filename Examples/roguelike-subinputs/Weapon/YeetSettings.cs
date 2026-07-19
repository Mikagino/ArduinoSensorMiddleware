using Godot;

namespace Weapon {
    [GlobalClass]
    public partial class YeetSettings : Resource {
        [Export] public int MinYeetRange = 400;
        [Export] public int MaxYeetRange = 600;
        [Export] public float YeetDurationScale = 0.002f;
    }
}