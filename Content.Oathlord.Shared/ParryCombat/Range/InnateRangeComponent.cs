namespace Content.Oathlord.Shared.ParryCombat.Range;

/// <summary>
/// Provides an innate range to a weapon that is always present.
/// Could be applied to a mob so it's unarmed melee attack has a certain range.
/// </summary>
[RegisterComponent]
public sealed partial class InnateRangeComponent : Component
{
    /// <summary>
    /// The titular range this entity innately has.
    /// </summary>
    [DataField]
    public float Range;

    /// <summary>
    /// When examining an entity with this component, it will talk about the range in reference to it being a weapon.
    /// If this component is on a mob then it would be really weird so you want to hide the examine.
    /// </summary>
    [DataField]
    public bool HideExamine = false;
}