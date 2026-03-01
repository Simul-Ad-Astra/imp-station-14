using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.RatKing;

public abstract class SharedDomainActionSystem : EntitySystem
{
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;
    [Dependency] protected readonly IRobustRandom Random = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<DomainActionComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<DomainActionComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, DomainActionComponent component, ComponentStartup args)
    {
        if (!TryComp(uid, out ActionsComponent? comp))
            return;

        _action.AddAction(uid, ref component.ActionDomainEntity, component.ActionDomain, component: comp);
    }

    private void OnShutdown(EntityUid uid, DomainActionComponent component, ComponentShutdown args)
    {

        if (!TryComp(uid, out ActionsComponent? comp))
            return;

        var actions = new Entity<ActionsComponent?>(uid, comp);
        _action.RemoveAction(actions, component.ActionDomainEntity);
    }
}

public sealed partial class RatKingDomainActionEvent : InstantActionEvent
{

}
