using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.ValueConverters;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public delegate SwSimpleKingdomMiniViewModel SwSimpleKingdomMiniViewModelFactory();

public class SwSimpleKingdomMiniViewModel : ViewModelBase
{
    private KingdomId _kingdom;
    protected readonly IOverrideDataProvider _spriteProvider;
    public SwSimpleKingdomMiniViewModel(IOverrideDataProvider spriteProvider)
    {
        _spriteProvider = spriteProvider;
    }

    public void Init(KingdomId kingdom)
    {
        _kingdom = kingdom;
        UpdateKingdomImage();
    }

    private void UpdateKingdomImage()
    {
        string spriteFile = _spriteProvider.GetSpriteFile(SpriteType.StlCastleIcon, (int)_kingdom).File;
        if (!PathToImageSourceConverter.TryConvert(spriteFile, out var img))
        {
            KingdomImage = null;
            return;
        }
        KingdomImage = img;
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
