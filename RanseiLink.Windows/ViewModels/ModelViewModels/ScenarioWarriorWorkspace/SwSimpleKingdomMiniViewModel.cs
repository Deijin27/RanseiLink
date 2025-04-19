#nullable enable
using RanseiLink.Core.Enums;

namespace RanseiLink.Windows.ViewModels;

public class SwSimpleKingdomMiniViewModel : BaseSimpleKingdomMiniViewModel
{
    public delegate SwSimpleKingdomMiniViewModel Factory();

    public SwSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider, IIdToNameService idToNameService)
        :base(spriteProvider, idToNameService)
    {
    }

    public SwSimpleKingdomMiniViewModel Init(KingdomId kingdom)
    {
        _kingdom = kingdom;

        return this;
    }

    public virtual int Army
    {
        get => 17;
        set { }
    }
}
