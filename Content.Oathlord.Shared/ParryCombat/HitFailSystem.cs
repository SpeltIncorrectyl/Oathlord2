using Content.Shared.Popups;

namespace Content.Oathlord.Shared.ParryCombat;

/// <summary>
/// A system that managages misses and parries (all generalised as a 'hitfail').
/// </summary>
public sealed partial class HitFailSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;

    /// <summary>
    /// Create a hit fail between an attacker and a defender.
    /// The attack will be penalised and the defender will be rewarded.
    /// </summary>
    public void CreateHitFail(EntityUid attacker, EntityUid defender, LocId message)
    {
        _popup.PopupPredictedCoordinates(Loc.GetString(message), Transform(defender).Coordinates, attacker, PopupType.MediumCaution);
    }
}