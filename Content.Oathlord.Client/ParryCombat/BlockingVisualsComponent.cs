using Robust.Shared.Utility;

namespace Content.Oathlord.Client.ParryCombat;

/// <summary>
/// This is used to configure the sprite layers for a mob so it has a graphic when blocking.
/// </summary>
[RegisterComponent]
public sealed partial class BlockingVisualsComponent : Component
{
    [DataField]
    public ResPath Sprite;

    [DataField]
    public string State;
}
