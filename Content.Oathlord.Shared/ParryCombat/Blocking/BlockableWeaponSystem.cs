using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Popups;
using Content.Shared.Wieldable.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Interaction.Events;
using Content.Shared.Hands;
using Content.Shared.Wieldable;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Oathlord.Shared.ParryCombat.Blocking;

public sealed partial class BlockableWeaponSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlockableWeaponRequiresWieldComponent, AttemptStartBlockingEvent>(OnAttemptWieldBlocking);
        SubscribeLocalEvent<BlockableWeaponComponent, AttemptMeleeEvent>(OnAttemptMelee);
        SubscribeLocalEvent<BlockableWeaponComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<BlockableWeaponRequiresWieldComponent, ItemUnwieldedEvent>(OnItemUnwielded);
        SubscribeLocalEvent<BlockableWeaponComponent, GotUnequippedHandEvent>(OnDropped);
        SubscribeLocalEvent<BlockableWeaponComponent, HandDeselectedEvent>(OnHandDeselected);
    }

    /// <summary>
    /// Can the user use this weapon to block?
    /// </summary>
    public bool CanBlock(EntityUid user, Entity<BlockableWeaponComponent> weapon)
    {
        if (!_hands.IsHolding(user, weapon.Owner))
            return false;
        
        var ev = new AttemptStartBlockingEvent(user);
        RaiseLocalEvent(weapon.Owner, ref ev);
        if (ev.Cancelled)
            return false;
        
        return true;
    }

    /// <summary>
    /// Start blocking with this weapon.
    /// </summary>
    public void StartBlocking(EntityUid user, Entity<BlockableWeaponComponent> weapon)
    {
        if (weapon.Comp.Blocking)
            return;

        weapon.Comp.Blocking = true;
        _appearance.SetData(user, BlockingVisuals.Shield, true);
        Dirty(weapon);
    }

    /// <summary>
    /// Try to start blocking with this weapon.
    /// Checks <see cref=CanBlock/>.
    /// Returns true if successful.
    /// </summary>
    public bool TryStartBlocking(EntityUid user, Entity<BlockableWeaponComponent> weapon)
    {
        if (!CanBlock(user, weapon))
            return false;
        
        StartBlocking(user, weapon);
        return true;
    }

    /// <summary>
    /// Stop blocking with this weapon.
    /// </summary>
    public void StopBlocking(EntityUid user, Entity<BlockableWeaponComponent> weapon)
    {
        if (!weapon.Comp.Blocking)
            return;

        weapon.Comp.Blocking = false;
        _appearance.SetData(user, BlockingVisuals.Shield, false);
        Dirty(weapon);
    }

    private void OnAttemptWieldBlocking(Entity<BlockableWeaponRequiresWieldComponent> entity, ref AttemptStartBlockingEvent args)
    {
        if (!TryComp<WieldableComponent>(entity.Owner, out var wieldable))
        {
            Log.Warning($"Entity {entity.Owner} has BlockableWeaponRequiresWieldComponent but not WieldableComponent.");
            args.Cancelled = true;
            return;
        }

        if (!wieldable.Wielded)
            args.Cancelled = true;
    }

    private void OnAttemptMelee(Entity<BlockableWeaponComponent> entity, ref AttemptMeleeEvent args)
    {
        if (entity.Comp.Blocking)
        {
            args.Message = Loc.GetString("blockable-weapon-popup-cannot-attack");
            args.Cancelled = true;
        }
    }

    private void OnUseInHand(Entity<BlockableWeaponComponent> entity, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;

        if (!entity.Comp.Blocking) {
            if (TryStartBlocking(args.User, entity))
            {
                args.Handled = true;
            }
        }
        else
        {
            StopBlocking(args.User, entity);
            args.Handled = true;
        }
    }

    private void OnItemUnwielded(Entity<BlockableWeaponRequiresWieldComponent> entity, ref ItemUnwieldedEvent args)
    {
        if (!TryComp<BlockableWeaponComponent>(entity.Owner, out var blockable))
        {
            Log.Warning($"Entity {entity.Owner} has BlockableWeaponRequiresWieldComponent but not BlockableWeaponComponent.");
            return;
        }

        StopBlocking(args.User, (entity.Owner, blockable));
    }

    private void OnDropped(Entity<BlockableWeaponComponent> entity, ref GotUnequippedHandEvent args)
    {
        if (entity.Owner == args.Unequipped)
            StopBlocking(args.User, entity);
    }

    private void OnHandDeselected(Entity<BlockableWeaponComponent> entity, ref HandDeselectedEvent args)
    {
        StopBlocking(args.User, entity);
    }
}

/// <summary>
/// Raised on a weapon when someone attempts to start blocking with it.
/// </summary>
[ByRefEvent]
public record struct AttemptStartBlockingEvent(EntityUid User, bool Cancelled = false);

[Serializable, NetSerializable, Flags]
public enum BlockingVisuals 
{
    Shield
}