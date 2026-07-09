using Robust.Client.GameObjects;
using Content.Oathlord.Shared.ParryCombat.Blocking;

namespace Content.Oathlord.Client.ParryCombat.Blocking;

/// <summary>
/// Configures the sprite layers for mobs which can block with weapons.
/// </summary>
public sealed partial class BlockingVisualizerSystem : EntitySystem
{
    [Dependency] private SpriteSystem _spriteSystem = default!;
    [Dependency] private AppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BlockingVisualsComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<BlockingVisualsComponent, AppearanceChangeEvent>(OnAppearanceChanged);
    }

    private void UpdateAppearance(EntityUid entity)
    {
        var visible = false;
        if (_appearance.TryGetData<bool>(entity, BlockingVisuals.Shield, out var blockingVisuals))
            visible = blockingVisuals;
        
        _spriteSystem.LayerSetVisible(entity, BlockingVisualsLayers.Shield, visible);
    }

    private void OnInit(Entity<BlockingVisualsComponent> entity, ref ComponentInit args)
    {
        _spriteSystem.LayerMapReserve(entity.Owner, BlockingVisualsLayers.Shield);
        _spriteSystem.LayerSetRsi(entity.Owner, BlockingVisualsLayers.Shield, entity.Comp.Sprite);
        _spriteSystem.LayerSetRsiState(entity.Owner, BlockingVisualsLayers.Shield, entity.Comp.State);
        UpdateAppearance(entity.Owner);
    }

    private void OnAppearanceChanged(Entity<BlockingVisualsComponent> entity, ref AppearanceChangeEvent args)
    {
        if (args.Sprite != null)
            UpdateAppearance(entity.Owner);
    }
}

public enum BlockingVisualsLayers
{
    Shield
}