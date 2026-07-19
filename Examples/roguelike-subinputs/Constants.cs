using Godot;

public partial class Constants : Node {
    public struct Inputs {
        public const string Up = "Up";
        public const string Down = "Down";
        public const string Left = "Left";
        public const string Right = "Right";
        public const string DropItem = "DropItem";
        public const string Shoot = "Shoot";
    }

    public struct Groups {
        public const string Player = "Player";
        public const string WeaponItem = "WeaponItem";
    }
}