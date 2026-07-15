using System;
using Godot;
using Player;
using Weapon;

public partial class PickupComponent : Area2D {
    [Signal] public delegate void PickedUpWeaponEventHandler(WeaponResource weapon);


    public override void _Ready() {
        AreaEntered += HandlePickup;
    }


    private void HandlePickup(Area2D area) {
        var item = area.GetParent();
        if(item is WeaponItem) {
            EmitSignal(SignalName.PickedUpWeapon, (item as WeaponItem).Weapon);
        }
        else {
            throw new NotImplementedException();
        }
    }
}
