using Content.Server._Impstation.Colonid.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Shared.Atmos;

namespace Content.Server._Impstation.Colonid.Components;

/// <summary>
///     This component ligts an entity on fire if it is not wearing clothing with the suitEVA or sealedClothing tags (sealedClothing was made specifically for this component)
///     AND the entity is in an atmosphere containing the specified gas.
/// </summary>
[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class IgniteFromGasComponent : Component
{
    /// <summary>
    ///     The amount of fire stacks that should be applied per check.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int FireStacksAmount = 2;

    /// <summary>
    ///     The gas that sets the entity on fire.
    ///     Defaults to oxygen
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public Gas TriggerGas;

    /// <summary>
    ///     The amount of gas needed to trigger ignition
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float TriggerThreshold;

    /// <summary>
    ///     The server time at which the next check for the triggering gas is done.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan NextCheck = TimeSpan.Zero;

    /// <summary>
    ///     How often the check for the triggering gas is proformed.
    ///     Defaults to every quarter of a second
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(0.25);
}
