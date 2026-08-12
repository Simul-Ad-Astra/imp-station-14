using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Impstation.Clothing;

/// <summary>
///     This component makes a piece of equipment take up multiple inventory slots with virtual items.
/// </summary>
[Access(typeof(MultiSlotClothingSystem))]
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MultiSlotClothingComponent : Component
{
    /// <summary>
    ///     The inventory slot flags required for this component to function.
    /// </summary>
    [DataField("requiredSlot"), AutoNetworkedField]
    public SlotFlags RequiredFlags = SlotFlags.INNERCLOTHING;

    /// <summary>
    ///     The inventory slot that the virtual item is equipped to.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField, AutoNetworkedField]
    public string Slot = "head";
}
