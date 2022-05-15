using RanseiLink.Core.Graphics;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services;
using RanseiLink.ValueConverters;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class BannerViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IBannerService _bannerService;
    private ImageSource _displayImage;
    private readonly BannerInfo _bannerInfo;
    private string _allTitles;

    public BannerViewModel(IDialogService dialogService, IBannerService bannerService)
    {
        _dialogService = dialogService;
        _bannerService = bannerService;
        _bannerInfo = _bannerService.BannerInfo;
        ReplaceImageCommand = new RelayCommand(ReplaceImage);
        SetAllTitlesCommand = new RelayCommand(SetAllTitles);
        UpdateDisplayImage();
    }

    public ICommand ReplaceImageCommand { get; }
    public ICommand SetAllTitlesCommand { get; }

    public string AllTitles
    {
        get => _allTitles;
        set => RaiseAndSetIfChanged(ref _allTitles, value);
    }

    public string JapaneseTitle
    {
        get => _bannerInfo.JapaneseTitle;
        set => RaiseAndSetIfChanged(JapaneseTitle, value, v => _bannerInfo.JapaneseTitle = value);
    }

    public string EnglishTitle
    {
        get => _bannerInfo.EnglishTitle;
        set => RaiseAndSetIfChanged(EnglishTitle, value, v => _bannerInfo.EnglishTitle = value);
    }

    public string FrenchTitle
    {
        get => _bannerInfo.FrenchTitle;
        set => RaiseAndSetIfChanged(FrenchTitle, value, v => _bannerInfo.FrenchTitle = value);
    }

    public string GermanTitle
    {
        get => _bannerInfo.GermanTitle;
        set => RaiseAndSetIfChanged(GermanTitle, value, v => _bannerInfo.GermanTitle = value);
    }

    public string ItalianTitle
    {
        get => _bannerInfo.ItalianTitle;
        set => RaiseAndSetIfChanged(ItalianTitle, value, v => _bannerInfo.ItalianTitle = value);
    }

    public string SpanishTitle
    {
        get => _bannerInfo.SpanishTitle;
        set => RaiseAndSetIfChanged(SpanishTitle, value, v => _bannerInfo.SpanishTitle = value);
    }

    public ImageSource DisplayImage
    {
        get => _displayImage;
        private set => RaiseAndSetIfChanged(ref _displayImage, value);
    }

    private void UpdateDisplayImage()
    {
        if (PathToImageSourceConverter.TryConvert(_bannerService.ImagePath, out var img))
        {
            DisplayImage = img;
        }
        else
        {
            DisplayImage = null;
        }
    }

    private void ReplaceImage()
    {
        if (!_dialogService.RequestFile("Choose a file to set as banner image", ".png", "PNG Image (.png)|*.png", out string file))
        {
            return;
        }

        if (!ImageSimplifier.ImageMatchesSize(file, 32, 32))
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                "Invalid dimensions",
                $"The dimensions of this image should be 32x32."
                ));
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

        if (ImageSimplifier.SimplifyPalette(file, 16, temp))
        {
            if (!_dialogService.SimplfyPalette(16, file, temp))
            {
                return;
            }
            file = temp;
        }

        _bannerService.SetImage(file);
        
        if (File.Exists(temp))
        {
            File.Delete(temp);
        }

        UpdateDisplayImage();
    }

    public void SetAllTitles()
    {
        string title = AllTitles;
        JapaneseTitle = title;
        EnglishTitle = title;
        FrenchTitle = title;
        GermanTitle = title;
        ItalianTitle = title;
        SpanishTitle = title;
    }
}
