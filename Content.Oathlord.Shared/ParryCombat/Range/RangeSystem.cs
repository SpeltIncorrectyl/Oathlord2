using Content.Shared.Wieldable.Components;
using Content.Shared.Examine;

namespace Content.Oathlord.Shared.ParryCombat.Range;

/// <summary>
/// A system that manages calculates the combat range of differnet weapons. Think sword vs spear.
/// </summary>
public sealed class RangeSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<InnateRangeComponent, GetRangeEvent>(OnGetRangeInnate);
        SubscribeLocalEvent<RangeOnWieldComponent, GetRangeEvent>(OnGetRangeWielded);
        SubscribeLocalEvent<InnateRangeComponent, ExaminedEvent>(OnExaminedInnate);
        SubscribeLocalEvent<RangeOnWieldComponent, ExaminedEvent>(OnExaminedWielded);
    }

    /// <summary>
    /// Gets the range of a weapon. If there is no component that sets this value then it defaults to 0.
    /// </summary>
    public float GetRange(EntityUid weapon)
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
}

[ByRefEvent]
public record struct GetRangeEvent(float Range = 0f, bool Handled = false);