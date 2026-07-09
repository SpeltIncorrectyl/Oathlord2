using Robust.Shared.GameStates;

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
}