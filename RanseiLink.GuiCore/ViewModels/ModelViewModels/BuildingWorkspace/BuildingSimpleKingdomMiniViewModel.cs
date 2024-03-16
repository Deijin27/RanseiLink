using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class BuildingSimpleKingdomMiniViewModel : ViewModelBase
{
    private readonly KingdomId _kingdom;
    protected readonly ICachedSpriteProvider _spriteProvider;
    public BuildingSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider, KingdomId kingdom)
    {
        _spriteProvider = spriteProvider;
        _kingdom = kingdom;
    }

    public object? KingdomImage => _spriteProvider.GetSprite(SpriteType.StlCastleIcon, (int)_kingdom);

    public KingdomId Kingdom => _kingdom;
}
