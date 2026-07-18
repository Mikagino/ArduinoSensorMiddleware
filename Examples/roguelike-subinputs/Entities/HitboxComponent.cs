using Godot;

[GlobalClass]
public partial class HitboxComponent : CollisionShape2D {
    [Export] public int MaxHealth = 5;
    [Export] public int CurrentHealth;


    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void DamageReceivedEventHandler(int currentHealth);


    public override void _Ready() {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Reduce current health and emit Died if it falls below 0.
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(int damage) {
        CurrentHealth -= damage;
        EmitSignal(SignalName.DamageReceived, CurrentHealth);
        if(CurrentHealth <= 0) {
            CurrentHealth = 0;
            EmitSignal(SignalName.Died);
        }
    }
}
