using Content.Server._Impstation.Colonid.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Server._Impstation.Colonid.Components;

/// <summary>
///     This component ligts an entity on fire if it is not wearing clothing with the suitEVA or sealedClothing tags (sealedClothing was made specifically for this component)
///     AND the entity is in an atmosphere containing the specified gas.
/// </summary>
[RegisterComponent, Access(typeof(IgniteFromGasSystem))]
public sealed partial class IgniteFromGasComponent : Component
{
    /// <summary>
    ///     the amount of fire stacks that should be applied per check.
    /// </summary>
    [DataField("fireStacksAmount"), ViewVariables(VVAccess.ReadWrite)]
    public int FireStacksAmount = 2;

    /// <summary>
    ///     the gas that sets the entity on fire
    /// </summary>
    [DataField("triggeringGas"), ViewVariables(VVAccess.ReadWrite)]
    public string TriggeringGas;

}
