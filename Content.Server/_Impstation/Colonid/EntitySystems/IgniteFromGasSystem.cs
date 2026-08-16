using Content.Server.Atmos.EntitySystems;
using Content.Server._Impstation.Colonid.Components;
using Content.Shared.Atmos;
using Content.Shared.Inventory;
using Robust.Shared.Timing;
using Content.Shared.Atmos.Components;

namespace Content.Server._Impstation.Colonid.EntitySystems;

public sealed class IgniteFromGasSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmo = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly FlammableSystem _flammable = default!;

    private float _timer = 0;
    /// <summary>
    ///     How often the check for the triggering gas is performed.
    /// </summary>
    public float UpdateInterval = 25f;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime) // honestly I don't really understand most of the stuff in here. I just copied it from the slarti guide. that's probably why it's not working.
    {                                            // ok I changed it to something else that I think I understand now
        _timer += frameTime;
        if (_timer < UpdateInterval)
            return;
        _timer -= UpdateInterval;

        var enumerator = EntityQueryEnumerator<IgniteFromGasComponent>();
        while (enumerator.MoveNext(out var uid, out var ignite))
        {
            Entity<IgniteFromGasComponent?> ent = uid;
            if (Comp<IgniteFromGasComponent>(ent) == null)
                return;
            if (CheckAtmosForGas(ent) && !CheckInventoryForProtection(ent))
            {
                if (Comp<FlammableComponent>(uid) == null)
                    return;

                _flammable.AdjustFireStacks(ent, ignite.FireStacksAmount);
            }
        }
    }

    /// <summary>
    ///     checks if the atmosphere the entity is in contains the gas specified in the component.
    /// </summary>
    /// <param name="entity">The entity with IgniteFromGasComponent</param>
    /// <returns> true or false </returns>
    private bool CheckAtmosForGas(Entity<IgniteFromGasComponent?> entity)
    {
        if (entity == null || entity.Comp == null)
            return false;

        TransformComponent? location = Transform(entity); // get the transformatiuon component of the entity.
        if (location == null)
            return false;

        GasMixture? gasMix = _atmo.GetTileMixture(location.Owner); // take the gasMixure of then tile the entity is one.

        if (gasMix == null) // make sure the gas mixture has gas in it
            return false;

        if (gasMix.GetMoles(entity.Comp.TriggerGas) < entity.Comp.TriggerThreshold) // if the amount of the trigger gas is below the trigger threshold
            return false;

        return true; // if all of that passed, then it must be true
    }

    /// <summary>
    ///     checks the entity's inventory to see if it's wearing protection in both the head slot and the inner/outer wear slot
    /// </summary>
    /// <param name="entity"></param>
    /// <returns> true or false </returns>
    private bool CheckInventoryForProtection(Entity<IgniteFromGasComponent?> ent) // TODO: find some way to let the component define what slots are necessary without breaking the logic.
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
            return true; // I could flip the logic on this one to have a return true at the end but I don't feel like doing that. yell at me if that's bad.

        return false; // if all else fails, return false
    }
}
