﻿#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class BannerViewModel : ViewModelBase
{
    private readonly IAsyncDialogService _dialogService;
    private readonly IBannerService _bannerService;
    private readonly IPathToImageConverter _pathToImageConverter;
    private object? _displayImage;
    private readonly BannerInfo _bannerInfo;
    private string _allTitles = string.Empty;

    public BannerViewModel(IAsyncDialogService dialogService, IBannerService bannerService, IPathToImageConverter pathToImageConverter)
    {
        _dialogService = dialogService;
        _bannerService = bannerService;
        _pathToImageConverter = pathToImageConverter;
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
        set => SetProperty(ref _allTitles, value ?? string.Empty);
    }

    public string JapaneseTitle
    {
        get => _bannerInfo.JapaneseTitle;
        set => SetProperty(JapaneseTitle, value, v => _bannerInfo.JapaneseTitle = value);
    }

    public string EnglishTitle
    {
        get => _bannerInfo.EnglishTitle;
        set => SetProperty(EnglishTitle, value, v => _bannerInfo.EnglishTitle = value);
    }

    public string FrenchTitle
    {
        get => _bannerInfo.FrenchTitle;
        set => SetProperty(FrenchTitle, value, v => _bannerInfo.FrenchTitle = value);
    }

    public string GermanTitle
    {
        get => _bannerInfo.GermanTitle;
        set => SetProperty(GermanTitle, value, v => _bannerInfo.GermanTitle = value);
    }

    public string ItalianTitle
    {
        get => _bannerInfo.ItalianTitle;
        set => SetProperty(ItalianTitle, value, v => _bannerInfo.ItalianTitle = value);
    }

    public string SpanishTitle
    {
        get => _bannerInfo.SpanishTitle;
        set => SetProperty(SpanishTitle, value, v => _bannerInfo.SpanishTitle = value);
    }

    public object? DisplayImage
    {
        get => _displayImage;
        private set => SetProperty(ref _displayImage, value);
    }

    private void UpdateDisplayImage()
    {
        DisplayImage = _pathToImageConverter.TryConvert(_bannerService.ImagePath);
    }

    private async Task ReplaceImage()
    {
        var file = await _dialogService.ShowOpenSingleFileDialog(new("Choose a file to set as banner image", new FileDialogFilter("PNG Image (.png)", ".png")));
        if (string.IsNullOrEmpty(file))
        {
            return;
        }

        if (!ImageSimplifier.ImageMatchesSize(file, 32, 32))
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                "Invalid dimensions",
                $"The dimensions of this image should be 32x32."
                ));
            return;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

        if (ImageSimplifier.SimplifyPalette(file, 16, temp))
        {
            var vm = new SimplifyPaletteViewModel(16, file, temp);
            if (!await _dialogService.ShowDialogWithResult(vm))
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

    private async Task ExportImage()
    {
        var dest = await _dialogService.ExportImage(Path.GetFileName(_bannerService.ImagePath));
        if (string.IsNullOrEmpty(dest))
        {
            return;
        }
        File.Copy(_bannerService.ImagePath, dest);
    }
}
