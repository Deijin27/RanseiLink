using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class SwSimpleKingdomMiniViewModel : ViewModelBase
{
    public delegate SwSimpleKingdomMiniViewModel Factory();

    private KingdomId _kingdom;
    protected readonly ICachedSpriteProvider _spriteProvider;
    public SwSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider)
    {
        _spriteProvider = spriteProvider;
    }

    public SwSimpleKingdomMiniViewModel Init(KingdomId kingdom)
    {
        _kingdom = kingdom;

        return this;
    }

    public ImageSource KingdomImage => _spriteProvider.GetSprite(SpriteType.StlCastleIcon, (int)_kingdom);

    public KingdomId Kingdom => _kingdom;

    public virtual int Army
    {
        get => 17;
        set { }
    }
}
