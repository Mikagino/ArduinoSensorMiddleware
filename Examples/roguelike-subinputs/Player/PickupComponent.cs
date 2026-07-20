using System;
using Godot;
using Weapon;

public partial class PickupComponent : Area2D {
    [Signal] public delegate void PickedUpWeaponEventHandler(WeaponItem weapon);


    public override void _Ready() {
        BodyEntered += HandlePickup;
    }


    private void HandlePickup(Node2D body) {
        GD.Print("Pickup of type: " + body.GetType());
        if(body is WeaponItem) {
            EmitSignal(SignalName.PickedUpWeapon, body as WeaponItem);
        }
        else {
            throw new NotImplementedException();
        }
    }
}
