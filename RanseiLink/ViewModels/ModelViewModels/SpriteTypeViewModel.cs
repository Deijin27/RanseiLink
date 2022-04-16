using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class SpriteTypeViewModel : ViewModelBase
{
    private readonly IOverrideSpriteProvider _spriteProvider;
    private readonly IDialogService _dialogService;
    private string _dimensionInfo;
    private bool _canAddNew;

    public SpriteTypeViewModel(IOverrideSpriteProvider overrideSpriteProvider, IDialogService dialogService)
    {
        _spriteProvider = overrideSpriteProvider;
        _dialogService = dialogService;

        AddNewCommand = new RelayCommand(AddNew, () => _canAddNew);
        ExportAllCommand = new RelayCommand(ExportAll);

        UpdateInfo(SelectedType);
        UpdateList();
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
                UpdateInfo(value);
            }
        } 
    }

    public string DimensionInfo
    {
        get => _dimensionInfo;
        set => RaiseAndSetIfChanged(ref _dimensionInfo, value);
    }

    public IReadOnlyCollection<IGraphicsInfo> SpriteTypeItems { get; } = GraphicsInfoResource.All;

    public ObservableCollection<SpriteItemViewModel> Items { get; private set; } = new ObservableCollection<SpriteItemViewModel>();

    public ICommand AddNewCommand { get; }
    public ICommand ExportAllCommand { get; }

    private void UpdateInfo(SpriteType type)
    {
        IGraphicsInfo info = GraphicsInfoResource.Get(type);
        _canAddNew = !info.FixedAmount;
        string dimensionInfo = "";
        if (info.Width != null)
        {
            dimensionInfo += $"width={info.Width} ";
        }
        if (info.Height != null)
        {
            dimensionInfo += $"height={info.Height} ";
        }
        dimensionInfo += $"palette-capacity={info.PaletteCapacity} ";
        DimensionInfo = dimensionInfo;
    }

    private void UpdateList()
    {
        _dialogService.ProgressDialog(delayOnCompletion: false, work:progress =>
        {
            progress.Report(new ProgressInfo("Loading..."));

            var files = _spriteProvider.GetAllSpriteFiles(SelectedType);
            progress.Report(new ProgressInfo(maxProgress: files.Count));
            int count = 0;
            List<SpriteItemViewModel> newItems = new();
            foreach (var i in files)
            {
                var item = new SpriteItemViewModel(i, _spriteProvider, _dialogService, this);
                newItems.Add(item);
                progress.Report(new ProgressInfo(progress: ++count));
            }
            Items = new ObservableCollection<SpriteItemViewModel>(newItems);
            RaisePropertyChanged(nameof(Items));
        });
    }

    private void AddNew()
    {
        if (_canAddNew)
        {
            var id = Items.Max(i => i.Id) + 1;
            if (SetOverride(id, $"Pick a file to add in slot '{id}' "))
            {
                var spriteFile = _spriteProvider.GetSpriteFile(SelectedType, id);
                var item = new SpriteItemViewModel(spriteFile, _spriteProvider, _dialogService, this);
                Items.Add(item);
            }
        }
    }

    public bool SetOverride(int id, string requestFileMsg)
    {
        if (!_dialogService.RequestFile(requestFileMsg, ".png", "PNG Image (.png)|*.png", out string file))
        {
            return false;
        }

        string temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");

        IGraphicsInfo gInfo = GraphicsInfoResource.Get(SelectedType);

        if (gInfo.Width != null && gInfo.Height != null)
        {
            int width = (int)gInfo.Width;
            int height = (int)gInfo.Height;

            if (!Core.Graphics.ImageSimplifier.ImageMatchesSize(file, width, height))
            {
                if (gInfo.StrictHeight || gInfo.StrictWidth)
                {
                    _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                        "Invalid dimensions",
                        $"The dimensions of this image should be {gInfo.Width}x{gInfo.Height}.\nFor this image type it is a strict requirement."
                        ));
                    return false;
                }

                var result = _dialogService.ShowMessageBox(new MessageBoxArgs(
                title: "Invalid dimensions",
                message: $"The dimensions of this image should be {gInfo.Width}x{gInfo.Height}.\nIf will work if they are different, but may look weird in game.",
                buttons: new[]
                {
                    new MessageBoxButton("Proceed anyway", MessageBoxResult.No),
                    new MessageBoxButton("Auto Resize", MessageBoxResult.Yes),
                    new MessageBoxButton("Cancel", MessageBoxResult.Cancel),
                },
                defaultResult: MessageBoxResult.Cancel
                ));

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Core.Graphics.ImageSimplifier.ResizeImage(file, width, height, temp);
                        file = temp;
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        return false;
                }
            }
        }

        if (Core.Graphics.ImageSimplifier.SimplifyPalette(file, gInfo.PaletteCapacity, temp))
        {
            if (!_dialogService.SimplfyPalette(gInfo.PaletteCapacity, file, temp))
            {
                return false;
            }
            file = temp;
        }

        _spriteProvider.SetOverride(SelectedType, id, file);

        if (File.Exists(temp))
        {
            File.Delete(temp);
        }

        return true;
    }

    private void ExportAll()
    {
        if (!_dialogService.RequestFolder("Select folder to export sprites into", out string dir))
        {
            return;
        }
        foreach (var spriteInfo in _spriteProvider.GetAllSpriteFiles(SelectedType))
        {
            string dest = Path.Combine(dir, Path.GetFileName(spriteInfo.File));
            File.Copy(spriteInfo.File, dest);
        }
        _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Export complete!", $"Sprites exported to: '{dir}'"));
    }
}
