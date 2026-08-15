using System.Linq;
using Content.Server.Atmos.EntitySystems;
using Content.Server._Impstation.Colonid.Components;
using Content.Shared.Atmos;
using Content.Shared.Inventory;
using static Content.Shared.Atmos.Components.GasAnalyzerComponent;

namespace Content.Server._Impstation.Colonid.EntitySystems;

public sealed class IgniteFromGasSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmo = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly FlammableSystem _flammable = default!;

    private readonly Entity<IgniteFromGasComponent> _ent = default;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (CheckAtmosForGas(_ent) && !CheckInventoryForProtection(_ent))
        {
            _flammable.AdjustFireStacks(_ent, _ent.Comp.FireStacksAmount);
        }

    }

    /// <summary>
    ///     checks if the atmosphere the entity is in contains the gas specified in the component.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns> true or false </returns>
    private bool CheckAtmosForGas(Entity<IgniteFromGasComponent> entity, Entity<TransformComponent?> ent)
    {
        string targetGas = entity.Comp.TriggeringGas;

        var gasAtTile = _atmo.GetContainingMixture(ent, true);
        var gasList = GenerateGasEntryArray(gasAtTile);

        for (var i = 0; i < gasList.Count; i++)
        {
            if (gasList[i] == targetGas)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     checks the entity's inventory to see if it's wearing protection in both the head slot and the inner/outer wear slot
    /// </summary>
    /// <param name="entity"></param>
    /// <returns> true or false </returns>
    private bool CheckInventoryForProtection(Entity<InventoryComponent> entity)
    {
        _inventory.TryGetSlotEntity(entity, "jumpsuit", out var jumpsuit);
        _inventory.TryGetSlotEntity(entity, "outerClothing", out var outer);
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

    private GasEntry[] GenerateGasEntryArray(GasMixture? mixture)
    {
        var gases = new List<GasEntry>();

        for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
        {
            var gas = _atmo.GetGas(i);

            if (mixture?[i] <= 0.01)
                continue;

            if (mixture != null)
            {
                var gasName = Loc.GetString(gas.Name);
                gases.Add(new GasEntry(gasName, mixture[i], gas.Color));
            }
        }

        var gasesOrdered = gases.OrderByDescending(gas => gas.Amount);

        return gasesOrdered.ToArray();
    }
}
