#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class SpriteItemViewModel : ViewModelBase
{
    public delegate SpriteItemViewModel Factory();

    private readonly IAsyncDialogService _dialogService;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly ISpriteManager _spriteManager;
    private SpriteType _spriteType;
    private bool _isOverride;
    private string _displayFile = null!;
    private object? _displayImage;

    public SpriteItemViewModel(ISpriteManager spriteManager, IOverrideDataProvider spriteProvider, IAsyncDialogService dialogService, IPathToImageConverter pathToImageConverter)
    {
        _dialogService = dialogService;
        _pathToImageConverter = pathToImageConverter;
        _spriteProvider = spriteProvider;
        _spriteManager = spriteManager;

        RevertCommand = new RelayCommand(Revert, () => _isOverride);
        ExportCommand = new RelayCommand(Export);
        SetOverrideCommand = new RelayCommand(SetOverride);
    }

    public SpriteItemViewModel Init(SpriteFile sprite)
    {
        Id = sprite.Id;

        _spriteType = sprite.Type;
        _isOverride = sprite.IsOverride;
        _displayFile = sprite.File;

        UpdateDisplayImage();

        return this;
    }

    public ICommand SetOverrideCommand { get; private set; }
    public ICommand ExportCommand { get; private set; }
    public RelayCommand RevertCommand { get; private set; }

    public int Id { get; private set; }
    public bool IsOverride => _isOverride;

    public object? DisplayImage
    {
        get => _displayImage;
        private set => SetProperty(ref _displayImage, value);  
    }

    private void UpdateDisplayImage()
    {
        DisplayImage = _pathToImageConverter.TryConvert(_displayFile);
    }

    private void Revert()
    {
        if (_isOverride)
        {
            _spriteProvider.ClearOverride(_spriteType, Id);
            _isOverride = false;
            RaisePropertyChanged(nameof(IsOverride));
            RevertCommand.RaiseCanExecuteChanged();
            _displayFile = _spriteProvider.GetSpriteFile(_spriteType, Id).File;
            if (File.Exists(_displayFile))
            {
                UpdateDisplayImage();
            }
            RaiseSpriteModified();
        }
    }

    private async Task Export()
    {
        var dir = await _dialogService.ShowOpenFolderDialog(new("Select folder to export sprite into"));
        if (string.IsNullOrEmpty(dir))
        {
            return;
        }
        var dest = FileUtil.MakeUniquePath(Path.Combine(dir, Path.GetFileName(_displayFile)));
        File.Copy(_displayFile, dest);
        await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Sprite Exported", $"Sprite exported to '{dest}'"));
    }

    private async void SetOverride()
    {
        if (await _spriteManager.SetOverride(_spriteType, Id, $"Pick a file to replace sprite '{Id}' with"))
        {
            var file = _spriteProvider.GetSpriteFile(_spriteType, Id);
            _displayFile = file.File;
            _isOverride = file.IsOverride;
            RaisePropertyChanged(nameof(IsOverride));
            RevertCommand.RaiseCanExecuteChanged();
            UpdateDisplayImage();
            RaiseSpriteModified();

        }
    }

    private void RaiseSpriteModified()
    {
        SpriteModified?.Invoke(this, _spriteProvider.GetSpriteFile(_spriteType, Id));
    }

    public event EventHandler<SpriteFile>? SpriteModified;
}
