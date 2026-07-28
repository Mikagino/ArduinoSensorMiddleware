using Godot;

public partial class StaminaBar : HBoxContainer {
    [Export] public int MaximumStamina = 5;


    public override void _Ready() {
        for(int i = 0; i < MaximumStamina - 1; i++)
            AddChild(GetChild(0).Duplicate());
    }


    public void SetStamina(int value) {
        for(int i = 0; i < value; i++) {
            GetChild<CanvasItem>(i).Show();
        }

        for(int i = value; i < MaximumStamina; i++) {
            GetChild<CanvasItem>(i).Hide();
        }
    }
}
