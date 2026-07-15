using Godot;

[GlobalClass]
public partial class HitboxComponent : CollisionShape2D {
    [Export] public int MaxHealth = 5;
    [Export] public int CurrentHealth;


    [Signal] public delegate void DiedEventHandler();


    public override void _Ready() {
        
    }

    /// <summary>
    /// Reduce current health and emit Died if it falls below 0.
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(int damage) {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0) {
            CurrentHealth = 0;
            EmitSignal(SignalName.Died);
        }
    }
}
