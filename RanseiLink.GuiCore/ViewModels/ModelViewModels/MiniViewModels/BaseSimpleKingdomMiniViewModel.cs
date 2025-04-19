using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public class BaseSimpleKingdomMiniViewModel : ViewModelBase
{
    protected KingdomId _kingdom;
    protected readonly ICachedSpriteProvider _spriteProvider;
    private readonly IIdToNameService _idToNameService;

    public BaseSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider, IIdToNameService idToNameService)
    {
        _spriteProvider = spriteProvider;
        _idToNameService = idToNameService;
    }

    public object? KingdomImage => _spriteProvider.GetSprite(SpriteType.StlCastleIcon, (int)_kingdom);

    public KingdomId Kingdom => _kingdom;

    public string KingdomName => _idToNameService.IdToName<IKingdomService>((int)Kingdom);
}
