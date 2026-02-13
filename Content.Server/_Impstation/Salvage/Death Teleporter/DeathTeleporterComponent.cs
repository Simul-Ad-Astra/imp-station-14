

namespace Content.Server._Impstation.Salvage;

[RegisterComponent]
[Access(typeof(SpeechRequiresEquipmentSystem))]
public sealed partial class DeathTeleporterComponent : Component
{
    /// <summary>
    /// Cooldown time between teleports.
    /// </summary>
    [ DataField, AutoPausedField ]
    public Timespan Cooldown = 123;

    /// <summary>
    /// Time before the entity is actually teleported.
    /// </summary>
    [ DataField, AutoPausedField ]
    public Timespan WarmUp = 2;

}
