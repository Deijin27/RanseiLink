using RanseiLink.Core;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RanseiLink.ViewModels;

public class SpriteItemViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IOverrideSpriteProvider _spriteProvider;
    private readonly SpriteType _spriteType;
    public SpriteItemViewModel(SpriteFile sprite, IOverrideSpriteProvider spriteProvider, IDialogService dialogService)
    {
        _dialogService = dialogService;
        _spriteProvider = spriteProvider;
        Id = sprite.Id;
        _spriteType = sprite.Type;
        _isOverride = sprite.IsOverride;
        _displayFile = sprite.File;

        RevertCommand = new RelayCommand(Revert, () => _isOverride);
        ExportCommand = new RelayCommand(Export);
        SetOverrideCommand = new RelayCommand(SetOverride);

        UpdateDisplayImage();
    }

    private bool _isOverride;

    public uint Id { get; }

    public string _displayFile;

    private ImageSource _displayImage;
    public ImageSource DisplayImage
    {
        get => _displayImage;
        set => RaiseAndSetIfChanged(ref _displayImage, value);  
    }

    private void UpdateDisplayImage()
    {
        using var fs = new FileStream(_displayFile, FileMode.Open, FileAccess.Read);
        if (fs.Length == 0)
        {
            return;
        }
        try
        {
            DisplayImage = BitmapFrame.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
        catch
        {
            DisplayImage = null;
        }
        
    }

    public event Action<SpriteItemViewModel> RemoveRequest;

    public ICommand SetOverrideCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand RevertCommand { get; }

    private void Revert()
    {
        if (_isOverride)
        {
            _spriteProvider.ClearOverride(_spriteType, Id);
            _isOverride = false;
            _displayFile = _spriteProvider.GetSpriteFilePath(_spriteType, Id);
            if (File.Exists(_displayFile))
            {
                UpdateDisplayImage();
            }
            else
            {
                RemoveRequest?.Invoke(this);
            }
        }
    }

    private void Export()
    {
        var dest = FileUtil.MakeUniquePath(Path.Combine(FileUtil.DesktopDirectory, Path.GetFileName(_displayFile)));
        File.Copy(_displayFile, dest);
        _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Sprite Exported", $"Sprite exported to '{dest}'"));
    }

    private void SetOverride()
    {
        if (!_dialogService.RequestFile($"Pick a file to replace sprite '{Id}' with", ".png", "PNG Image (.png)|*.png", out string file))
        {
            return;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
        bool usedTemp = false;
        if (Core.Graphics.PaletteSimplifier.SimplifyPalette(file, 256, temp))
        {
            usedTemp = true;
            if (!_dialogService.SimplfyPalette(256, file, temp))
            {
                return;
            }
            file = temp;
        }

        _spriteProvider.SetOverride(_spriteType, Id, file);
        _displayFile = _spriteProvider.GetSpriteFilePath(_spriteType, Id);
        UpdateDisplayImage();
        _isOverride = true;
        if (usedTemp)
        {
            File.Delete(temp);
        }
    }
}

public class SpriteTypeViewModel : ViewModelBase, ISaveableRefreshable
{
    private readonly IOverrideSpriteProvider _spriteProvider;
    private readonly IDialogService _dialogService;

    public SpriteTypeViewModel(IServiceContainer container, IEditorContext context)
    {
        _spriteProvider = context.DataService.OverrideSpriteProvider;
        _dialogService = container.Resolve<IDialogService>();

        AddNewCommand = new RelayCommand(AddNew);
        ExportAllCommand = new RelayCommand(ExportAll);
    }

    private SpriteType _selectedType = SpriteType.StlBushouB;
    public SpriteType SelectedType 
    {
        get => _selectedType;
        set
        {
            if (RaiseAndSetIfChanged(ref _selectedType, value))
            {
                UpdateList();
            }
        } 
    }

    public IReadOnlyCollection<IGraphicsInfo> SpriteTypeItems { get; } = GraphicsInfoResource.All;

    private List<SpriteItemViewModel> _items;
    public List<SpriteItemViewModel> Items
    {
        get => _items;
        set => RaiseAndSetIfChanged(ref _items, value);
    }

    private void UpdateList()
    {
        _dialogService.ProgressDialog(delayOnCompletion: true, work:progress =>
        {
            var newItems = new List<SpriteItemViewModel>();

            var files = _spriteProvider.GetAllSpriteFiles(SelectedType);
            progress.Report(new ProgressInfo("Loading...", MaxProgress: files.Count));
            int count = 0;
            foreach (var i in files)
            {
                var item = new SpriteItemViewModel(i, _spriteProvider, _dialogService);
                newItems.Add(item);
                item.RemoveRequest += _ => Refresh();
                progress.Report(new ProgressInfo(Progress: ++count));
            }

            progress.Report(new ProgressInfo("Updating UI..."));
            Items = newItems;
        });
    }

    public ICommand AddNewCommand { get; }
    public ICommand ExportAllCommand { get; }

    private void AddNew()
    {
        var id = Items.Max(i => i.Id) + 1;
        if (!_dialogService.RequestFile($"Pick a file to add in slot '{id}' ", ".png", "PNG Image (.png)|*.png", out string file))
        {
            return;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
        bool usedTemp = false;
        if (Core.Graphics.PaletteSimplifier.SimplifyPalette(file, 256, temp))
        {
            usedTemp = true;
            if (!_dialogService.SimplfyPalette(256, file, temp))
            {
                return;
            }
            file = temp;
        }

        _spriteProvider.SetOverride(SelectedType, id, file);
        UpdateList();

        if (usedTemp)
        {
            File.Delete(temp);
        }
    }

    private void ExportAll()
    {
        string dir = FileUtil.MakeUniquePath(Path.Combine(FileUtil.DesktopDirectory, SelectedType.ToString()));
        Directory.CreateDirectory(dir);
        foreach (var spriteInfo in _spriteProvider.GetAllSpriteFiles(SelectedType))
        {
            string dest = Path.Combine(dir, Path.GetFileName(spriteInfo.File));
            File.Copy(spriteInfo.File, dest);
        }
        _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Export complete!", $"Sprites exported to: '{dir}'"));
    }

    public void Refresh()
    {
        UpdateList();
    }

    public void Save()
    {
    }
}
