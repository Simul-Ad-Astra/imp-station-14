using System.Numerics;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.NPC;
using Content.Server.NPC.HTN;
using Content.Server.NPC.Systems;
using Content.Server.Popups;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Dataset;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Pointing;
using Content.Shared.Random.Helpers;
using Content.Shared.RatKing;
using Robust.Shared.Map;
using Content.Shared.Body.Components; // imp
using Content.Shared.Damage.Components; // imp
using Content.Shared.SubFloor; // imp
using Robust.Shared.Containers; // imp

namespace Content.Server.RatKing
{
    /// <inheritdoc/>
    public sealed class DomainActionSystem : SharedDomainActionSystem
    {
        [Dependency] private readonly AtmosphereSystem _atmos = default!;
        // [Dependency] private readonly ChatSystem _chat = default!;
        // [Dependency] private readonly HTNSystem _htn = default!;
        [Dependency] private readonly HungerSystem _hunger = default!;
        // [Dependency] private readonly NPCSystem _npc = default!;
        [Dependency] private readonly PopupSystem _popup = default!;
        // [Dependency] private readonly EntityLookupSystem _lookup = default!; // imp
        // [Dependency] private readonly SharedContainerSystem _container = default!; // imp

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<DomainActionComponent, RatKingDomainActionEvent>(OnDomain);
        }

        /// <summary>
        /// uses hunger to release a specific amount of ammonia into the air. This heals the rat king
        /// and his servants through a specific metabolism.
        /// </summary>
        private void OnDomain(EntityUid uid, DomainActionComponent component, RatKingDomainActionEvent args)
        {
            if (args.Handled)
                return;

            if (!TryComp<HungerComponent>(uid, out var hunger))
                return;

            //make sure the hunger doesn't go into the negatives
            if (_hunger.GetHunger(hunger) < component.HungerPerDomainUse)
            {
                _popup.PopupEntity(Loc.GetString("rat-king-too-hungry"), uid, uid);
                return;
            }
            args.Handled = true;
            _hunger.ModifyHunger(uid, -component.HungerPerDomainUse, hunger);

            _popup.PopupEntity(Loc.GetString("rat-king-domain-popup"), uid);
            var tileMix = _atmos.GetTileMixture(uid, excite: true);
            tileMix?.AdjustMoles(Gas.Ammonia, component.MolesAmmoniaPerDomain);
        }
    }
}
