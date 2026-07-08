using Content.Shared.Wieldable.Components;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Oathlord.Shared.ParryCombat;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Oathlord.Shared.ParryCombat.Range;

/// <summary>
/// A system that manages calculates the combat range of differnet weapons. Think sword vs spear.
/// </summary>
public sealed partial class RangeSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private HitFailSystem _hitFail = default!;

    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<InnateRangeComponent, GetRangeEvent>(OnGetRangeInnate);
        SubscribeLocalEvent<RangeOnWieldComponent, GetRangeEvent>(OnGetRangeWielded);
        SubscribeLocalEvent<InnateRangeComponent, ExaminedEvent>(OnExaminedInnate);
        SubscribeLocalEvent<RangeOnWieldComponent, ExaminedEvent>(OnExaminedWielded);
        SubscribeLocalEvent<RespectRangeComponent, MeleeHitEvent>(OnHit);
    }

    /// <summary>
    /// Gets the range of a weapon. If there is no component that sets this value then it defaults to 0.
    /// Unarmed attacks done by a mob are based on a melee weapon component attached to that mob.
    /// If you want to see the innate range of a mob then treat it as a weapon and pass it into this method.
    /// </summary>
    public float GetRangeOfWeapon(EntityUid weapon)
    {
        var ev = new GetRangeEvent();
        RaiseLocalEvent(weapon, ref ev);

        if (ev.Range < 0f)
        {
            Log.Warning($"Entity {weapon} has a negative range.");
            return 0f;
        }

        return ev.Range;
    }

    /// <summary>
    /// Finds the range of a mob, based on its innate range and then the range of everything its holding.
    /// The longest range is returned. Range does not stack.
    /// </summary>
    public float GetRangeOfDefender(EntityUid defender)
    {
        var bestRange = GetRangeOfWeapon(defender);
        foreach (var hand in _hands.EnumerateHands(defender))
        {
            if (!_hands.TryGetHeldItem(defender, hand, out var heldEntity))
                continue;

            var newRange = GetRangeOfWeapon(heldEntity.Value);
            if (newRange > bestRange)
                bestRange = newRange;
        }
        return bestRange;
    }

    private void OnGetRangeInnate(Entity<InnateRangeComponent> ent, ref GetRangeEvent args)
    {
        if (args.Handled)
            return;
        
        args.Range = ent.Comp.Range;
        args.Handled = true;
    }

    private void OnGetRangeWielded(Entity<RangeOnWieldComponent> ent, ref GetRangeEvent args)
    {
        if (args.Handled)
            return;
        
        args.Range = ent.Comp.UnwieldedRange;
        args.Handled = true;

        if (!TryComp<WieldableComponent>(ent.Owner, out var wieldable))
            return;
        
        if (wieldable.Wielded)
            args.Range = ent.Comp.WieldedRange;
    }

    private void OnExaminedInnate(Entity<InnateRangeComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.HideExamine)
            return;
        
        args.PushMarkup(Loc.GetString("range-examine-innate", ("range", ent.Comp.Range)));
    }

    private void OnExaminedWielded(Entity<RangeOnWieldComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.HideExamine)
            return;
        
        args.PushMarkup(Loc.GetString("range-examine-wielded", ("range", ent.Comp.WieldedRange)));
        if (ent.Comp.UnwieldedRange > 0f)
            args.PushMarkup(Loc.GetString("range-examine-unwielded", ("range", ent.Comp.WieldedRange)));
    }

    private void OnHit(Entity<RespectRangeComponent> ent, ref MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;
        if (args.Handled)
            return;
        
        foreach (var target in args.HitEntities)
        {
            if (GetRangeOfWeapon(args.Weapon) < GetRangeOfDefender(target))
            {
                _hitFail.CreateHitFail(args.User, target, "hitfail-out-of-range");
                args.Handled = true;
            }
        }
    }
}

[ByRefEvent]
public record struct GetRangeEvent(float Range = 0f, bool Handled = false);