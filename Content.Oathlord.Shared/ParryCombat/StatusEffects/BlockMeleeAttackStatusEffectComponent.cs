namespace Content.Oathlord.Shared.ParryCombat.StatusEffects;

[RegisterComponent]
public sealed partial class BlockMeleeAttackStatusEffectComponent : Component
{
    /// <summary>
    /// Message to display when you try and fail to attack because of this status effect.
    /// </summary>
    [DataField]
    public LocId Message;
}