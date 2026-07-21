using System;
using Godot;
using Weapon;

public partial class PickupComponent : Area2D {
    [Signal] public delegate void PickedUpWeaponEventHandler(WeaponItem weapon);


    public override void _Ready() {
        AreaEntered += HandlePickup;
    }


    private void HandlePickup(Area2D area) {
        Node2D areasParent = area.GetParent<Node2D>();
        if(areasParent is WeaponItem) {
            EmitSignal(SignalName.PickedUpWeapon, areasParent as WeaponItem);
        }
        else {
            throw new NotImplementedException();
        }
    }
}
