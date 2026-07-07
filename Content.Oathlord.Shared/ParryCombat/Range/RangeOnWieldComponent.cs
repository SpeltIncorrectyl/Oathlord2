namespace Content.Oathlord.Shared.ParryCombat.Range;

/// <summary>
/// Provides a range for a weapon when it is wielded, and another for when it is not.
/// Spears and swords have no range when not wielded.
/// </summary>
[RegisterComponent]
public sealed partial class RangeOnWieldComponent : Component
{
    /// <summary>
    /// The range when wielded.
    /// </summary>
    [DataField]
    public float WieldedRange;

    /// <summary>
    /// The range when not wielded.
    /// </summary>
    [DataField]
    public float UnwieldedRange = 0f;

    /// <summary>
    /// When examining an entity with this component, it will talk about the range in reference to it being a weapon.
    /// If this component is on a mob then it would be really weird so you want to hide the examine.
    /// </summary>
    [DataField]
    public bool HideExamine = false;
}