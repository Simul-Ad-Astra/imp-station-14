using System.Linq;
using Content.Server.Atmos.EntitySystems;
using Content.Server._Impstation.Colonid.Components;
using Content.Shared.Atmos;
using Content.Shared.Inventory;
using static Content.Shared.Atmos.Components.GasAnalyzerComponent;
using Robust.Shared.Timing;

namespace Content.Server._Impstation.Colonid.EntitySystems;

public sealed class IgniteFromGasSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmo = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly FlammableSystem _flammable = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly GasMixture _mixture = default!;

    private readonly Entity<IgniteFromGasComponent> _ent = default;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;



        if (CheckAtmosForGas(_ent) && !CheckInventoryForProtection(_ent))
        {
            _flammable.AdjustFireStacks(_ent, _ent.Comp.FireStacksAmount);
        }

    }

    /// <summary>
    ///     checks if the atmosphere the entity is in contains the gas specified in the component.
    /// </summary>
    /// <param name="entity">The entity with IgniteFromGasComponent</param>
    /// <returns> true or false </returns>
    private bool CheckAtmosForGas(Entity<IgniteFromGasComponent> entity)
    {
        TryComp<TransformComponent>(entity, out var location);
        if (location == null)
            return false;

        GasMixture? gasMix = _atmo.GetTileMixture(location.Owner); // take the gasMixure of then tile the entity is one

        if (gasMix == null)
            return false;

        if (_mixture.GetMoles(entity.Comp.TriggeringGas) >= entity.Comp.TriggerThreshold)
            return true;

        return false; // if all else fails return false.
    }

    /// <summary>
    ///     checks the entity's inventory to see if it's wearing protection in both the head slot and the inner/outer wear slot
    /// </summary>
    /// <param name="entity"></param>
    /// <returns> true or false </returns>
    private bool CheckInventoryForProtection(Entity<IgniteFromGasComponent> ent) // TODO: find some way to let the component define what slots are necessary without breaking the logic.
    {
        TryComp<InventoryComponent>(ent, out var inventory); // make sure the entity has an inventory to check
        if (inventory == null)
            return false;
        Entity<InventoryComponent?> entity = inventory.Owner;

        _inventory.TryGetSlotEntity(entity, "jumpsuit", out var jumpsuit); // I don't love hardcoding the necessary slots like this, but I'm not sure of a good way to do this without breaking the logic.
        _inventory.TryGetSlotEntity(entity, "outerClothing", out var outer); // i.e. while preserving needing either the jumpsuit OR outerclothing but not both, but always requiring the head.
        _inventory.TryGetSlotEntity(entity, "head", out var head);

        if (jumpsuit == null && outer == null) // if there isn't a jumpsuit OR outerclothing equipped
            return false;
        if (head == null) // if there isn't anything equipped in the head slot
            return false;

        // if neither [both jumpsuit and head] nor [both outerclothing and head] have SealedClothingComponent
        if ((TryComp<SealedClothingComponent>(jumpsuit, out var resultjump) || TryComp<SealedClothingComponent>(outer, out var resultouter)) && TryComp<SealedClothingComponent>(head, out var resulthead)) // I don't care about the result vars, but it won't let me just not have them.
            return true;

        return false; // if all else fails, return false
    }
}
