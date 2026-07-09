using Content.Shared.Hands.EntitySystems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands;
using Content.Shared.Weapons.Melee.Events;
using Content.Oathlord.Shared.Weapons.Melee;

namespace Content.Oathlord.Shared.Hands;

public sealed partial class OathlordHandsRelaySystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;
    
    public override void Initialize()
    {
        base.Initialize();

    }

    private void RelayEvent<T>(Entity<HandsComponent> entity, ref T args)
    {
        var ev = new HeldRelayedEvent<T>(args);
        foreach (var hand in _hands.EnumerateHeld(entity.AsNullable()))
        {
            RaiseLocalEvent(hand, ref ev);
        }
        args = ev.Args;
    }
}