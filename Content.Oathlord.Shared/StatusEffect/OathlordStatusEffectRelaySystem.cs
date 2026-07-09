using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Oathlord.Shared.StatusEffect;

public sealed partial class OathlordStatusEffectRelaySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusEffectContainerComponent, AttemptMeleeEvent>(RelayEvent);
    }

    private void RelayEvent<T>(Entity<StatusEffectContainerComponent> entity, ref T args) where T : struct
    {
        var ev = new StatusEffectRelayedEvent<T>(args);
        foreach (var activeEffect in entity.Comp.ActiveStatusEffects?.ContainedEntities ?? [])
        {
            RaiseLocalEvent(activeEffect, ref ev);
        }
        args = ev.Args;
    }
}