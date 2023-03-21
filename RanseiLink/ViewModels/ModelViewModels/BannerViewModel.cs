#nullable enable
using RanseiLink.Core;
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
    private ImageSource? _displayImage;
    private readonly BannerInfo _bannerInfo;
    private string _allTitles = string.Empty;

    public BannerViewModel(IDialogService dialogService, IBannerService bannerService)
    {
        _dialogService = dialogService;
        _bannerService = bannerService;
        _bannerInfo = _bannerService.BannerInfo;
        ReplaceImageCommand = new RelayCommand(ReplaceImage);
        SetAllTitlesCommand = new RelayCommand(SetAllTitles);
        ExportImageCommand = new RelayCommand(ExportImage, () => File.Exists(_bannerService.ImagePath));
        UpdateDisplayImage();
    }

    public ICommand ReplaceImageCommand { get; }
    public ICommand ExportImageCommand { get; }
    public ICommand SetAllTitlesCommand { get; }

    public string AllTitles
    {
        get => _allTitles;
        set => RaiseAndSetIfChanged(ref _allTitles, value ?? string.Empty);
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

    public ImageSource? DisplayImage
    {
        get => _displayImage;
        private set => RaiseAndSetIfChanged(ref _displayImage, value);
    }

    private void UpdateDisplayImage()
    {
        DisplayImage = PathToImageSourceConverter.TryConvert(_bannerService.ImagePath);
    }

    private void ReplaceImage()
    {
        var file = _dialogService.ShowOpenSingleFileDialog(new("Choose a file to set as banner image", new FileDialogFilter("PNG Image (.png)", ".png")));
        if (string.IsNullOrEmpty(file))
        {
            return;
        }

        if (!ImageSimplifier.ImageMatchesSize(file, 32, 32))
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                "Invalid dimensions",
                $"The dimensions of this image should be 32x32."
                ));
            return;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

        if (ImageSimplifier.SimplifyPalette(file, 16, temp))
        {
            var vm = new SimplifyPaletteViewModel(16, file, temp);
            if (!_dialogService.ShowDialogWithResult(vm))
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

    private void ExportImage()
    {
        var dir = _dialogService.ShowOpenFolderDialog(new("Select folder to export image into"));
        if (string.IsNullOrEmpty(dir))
        {
            return;
        }
        var dest = FileUtil.MakeUniquePath(Path.Combine(dir, Path.GetFileName(_bannerService.ImagePath)));
        File.Copy(_bannerService.ImagePath, dest);
        _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Image Exported", $"Exported to '{dest}'"));
    }
}
