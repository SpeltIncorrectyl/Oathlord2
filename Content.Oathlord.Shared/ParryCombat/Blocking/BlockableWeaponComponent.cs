using Robust.Shared.GameStates;
using Content.Shared.EntityEffects;

namespace Content.Oathlord.Shared.ParryCombat.Blocking;

/// <summary>
/// On a weapon which someone can use to block.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BlockableWeaponComponent : Component
{
    /// <summary>
    /// If the weapon is currently being used to block.
    /// </summary
    [DataField, AutoNetworkedField]
    public bool Blocking = false;

    /// <summary>
    /// Entity effects to apply to the attacker when an attack is successfully blocked.
    /// </summary
    [DataField]
    public EntityEffect[] EffectsOnBlockForAttacker = [];

    /// <summary>
    /// The scale for the attacker effects.
    /// If you want better shield to stagger for longer, for example, you can inherit prototype and just change the scale.
    /// </summary
    [DataField]
    public float EffectsOnBlockForAttackerScale = 1f;

    /// <summary>
    /// Entity effects to apply to the defender when an attack is successfully blocked.
    /// Defender is the person who was doing the blocking.
    /// </summary
    [DataField]
    public EntityEffect[] EffectsOnBlockForDefender = [];

    /// <summary>
    /// The scale for the defender effects.
    /// If you want better shield to provide the opportunity for repose for longer, for example, you can inherit prototype and just change the scale.
    /// </summary
    [DataField]
    public float EffectsOnBlockForDefenderScale = 1f;
}