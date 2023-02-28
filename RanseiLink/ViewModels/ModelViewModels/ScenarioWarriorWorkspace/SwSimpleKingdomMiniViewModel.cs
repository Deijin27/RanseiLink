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
        UpdateKingdomImage();

        return this;
    }

    private void UpdateKingdomImage()
    {
        KingdomImage = _spriteProvider.GetSprite(SpriteType.StlCastleIcon, (int)_kingdom);
    }

    private ImageSource _KingdomImage;
    public ImageSource KingdomImage
    {
        get => _KingdomImage;
        set => RaiseAndSetIfChanged(ref _KingdomImage, value);
    }

    public KingdomId Kingdom => _kingdom;

    public virtual int Army
    {
        get => 17;
        set { }
    }
}
