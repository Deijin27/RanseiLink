using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using RanseiLink.ValueConverters;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public delegate SpriteItemViewModel SpriteItemViewModelFactory(SpriteFile sprite);

public class SpriteItemViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly ISpriteManager _spriteManager;
    private readonly SpriteType _spriteType;
    private bool _isOverride;
    private string _displayFile;
    private ImageSource _displayImage;

    public SpriteItemViewModel(SpriteFile sprite, ISpriteManager spriteManager, IOverrideDataProvider spriteProvider, IDialogService dialogService)
    {
        _dialogService = dialogService;
        _spriteProvider = spriteProvider;
        _spriteManager = spriteManager;
        Id = sprite.Id;
        _spriteType = sprite.Type;
        _isOverride = sprite.IsOverride;
        _displayFile = sprite.File;

        RevertCommand = new RelayCommand(Revert, () => _isOverride);
        ExportCommand = new RelayCommand(Export);
        SetOverrideCommand = new RelayCommand(SetOverride);

        UpdateDisplayImage();
    }

    public ICommand SetOverrideCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand RevertCommand { get; }

    public int Id { get; }
    public bool IsOverride => _isOverride;

    public ImageSource DisplayImage
    {
        get => _displayImage;
        private set => RaiseAndSetIfChanged(ref _displayImage, value);  
    }

    private void UpdateDisplayImage()
    {
        if (PathToImageSourceConverter.TryConvert(_displayFile, out var img))
        {
            DisplayImage = img;
        }
        else
        {
            DisplayImage = null;
        }
    }

    private void Revert()
    {
        if (_isOverride)
        {
            _spriteProvider.ClearOverride(_spriteType, Id);
            _isOverride = false;
            RaisePropertyChanged(nameof(IsOverride));
            _displayFile = _spriteProvider.GetSpriteFile(_spriteType, Id).File;
            if (File.Exists(_displayFile))
            {
                UpdateDisplayImage();
            }
            RaiseSpriteModified();
        }
    }

    private void Export()
    {
        if (!_dialogService.RequestFolder("Select folder to export sprite into", out string dir))
        {
            return;
        }
        var dest = FileUtil.MakeUniquePath(Path.Combine(dir, Path.GetFileName(_displayFile)));
        File.Copy(_displayFile, dest);
        _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Sprite Exported", $"Sprite exported to '{dest}'"));
    }

    private void SetOverride()
    {
        if (_spriteManager.SetOverride(_spriteType, Id, $"Pick a file to replace sprite '{Id}' with"))
        {
            var file = _spriteProvider.GetSpriteFile(_spriteType, Id);
            _displayFile = file.File;
            _isOverride = file.IsOverride;
            RaisePropertyChanged(nameof(IsOverride));
            UpdateDisplayImage();
            RaiseSpriteModified();

        }
    }

    private void RaiseSpriteModified()
    {
        SpriteModified?.Invoke(this, new SpriteFile(_spriteType, Id, _displayFile, IsOverride));
    }

    public event EventHandler<SpriteFile> SpriteModified;
}
