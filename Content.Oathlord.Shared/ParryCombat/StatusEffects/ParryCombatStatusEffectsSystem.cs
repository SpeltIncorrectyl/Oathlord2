using Content.Shared.StatusEffectNew;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Oathlord.Shared.ParryCombat.StatusEffects;

public sealed partial class ParryCombatStatusEffectsSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlockMeleeAttackStatusEffectComponent, StatusEffectRelayedEvent<AttemptMeleeEvent>>(OnAttemptMelee);   
    }

    private void OnAttemptMelee(Entity<BlockMeleeAttackStatusEffectComponent> entity, ref StatusEffectRelayedEvent<AttemptMeleeEvent> args)
    {
        // got to do this weird stuff
        var ev = args.Args;
        ev.Cancelled = true;
        ev.Message = Loc.GetString(entity.Comp.Message);
        args.Args = ev;
    }
}