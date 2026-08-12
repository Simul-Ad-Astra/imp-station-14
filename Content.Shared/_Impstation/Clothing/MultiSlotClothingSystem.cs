using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Popups;
using Content.Shared.Strip;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._Impstation.Clothing;

public sealed class MultiSlotClothingSystem : EntitySystem
{
    [Dependency] private readonly ClothingSystem _clothingSystem = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly SharedVirtualItemSystem _virtualItem = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MultiSlotClothingComponent, ClothingGotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<MultiSlotClothingComponent, ClothingGotUnequippedEvent>(OnUnequip);
    }

    private void OnEquipped(Entity<MultiSlotClothingComponent> ent, ref ClothingGotEquippedEvent args)
    {
        if (args.Clothing.InSlot == null) // if the clothing isn't in a slot
            return;
        if (_inventorySystem.TryGetSlotEntity(args.Wearer, ent.Comp.Slot, out var existing)) // if the slot for the virtual item is already full
        {
            _inventorySystem.TryUnequip(args.Wearer, args.Clothing.InSlot); // try to unequip the clothing
            _popup.PopupClient(Loc.GetString("toggleable-clothing-remove-first", ("entity", existing)), ent.Owner);// and send a popup
            return;
        }
        if (!_virtualItem.TrySpawnVirtualItemInInventory(ent.Owner, args.Wearer, ent.Comp.Slot, true)) // try to make the virtual item
        {
            _inventorySystem.TryUnequip(args.Wearer, args.Clothing.InSlot); // if it fails unequip the item
            return;
        }
    }

    private void OnUnequip(Entity<MultiSlotClothingComponent> ent, ref ClothingGotUnequippedEvent args)
    {
        _virtualItem.DeleteInSlotMatching(args.Wearer, ent.Owner, ent.Comp.Slot); // delete the virtual item
    }
}
