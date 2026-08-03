using System.Linq;
using Godot;

public partial class StaminaBar : HBoxContainer {
    [Export] public int MaximumStamina = 5;
    [Export] public int CurrentStamina;


    public void InitializeStamina(int maximumStamina) {
        MaximumStamina = maximumStamina;
        CurrentStamina = maximumStamina;
        for(int i = 0; i < MaximumStamina - 1; i++)
            AddChild(GetChild<CanvasItem>(0).Duplicate());
    }


    public void SetStamina(int value) {
        CurrentStamina = Mathf.Clamp(value, 0, MaximumStamina);
        CanvasItem[] children = [.. GetChildren().OfType<CanvasItem>()];
        for(int i = 0; i < CurrentStamina; i++) {
            children[i].Show();
            GD.Print("Show");
        }

        for(int i = CurrentStamina; i < MaximumStamina; i++) {
            children[i].Hide();
            GD.Print("Hide");
        }

    }


    public void ReduceStamina(int value = 1) {
        SetStamina(CurrentStamina - value);
    }


    public void IncreaseStamina(int value = 1) {
        SetStamina(CurrentStamina + value);
    }
}
