#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Windows.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.Windows.ViewModels;

public class SpriteTypeViewModel : ViewModelBase
{
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IDialogService _dialogService;
    private readonly ISpriteManager _spriteManager;
    private string _dimensionInfo = null!;
    private bool _canAddNew;

    public SpriteTypeViewModel(ISpriteManager spriteManager, IOverrideDataProvider overrideSpriteProvider, IDialogService dialogService, SpriteItemViewModel.Factory spriteItemVmFactory)
    {
        _spriteProvider = overrideSpriteProvider;
        _dialogService = dialogService;
        _spriteManager = spriteManager;
        _spriteItemVmFactory = spriteItemVmFactory;

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

    public void UpdateInfo(SpriteType type)
    {
        IGraphicsInfo basicInfo = GraphicsInfoResource.Get(type);

        _canAddNew = !basicInfo.FixedAmount;

        if (basicInfo is IGroupedGraphicsInfo info)
        {
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

            int numberOverriden = _spriteProvider.GetAllSpriteFiles(SelectedType).Where(x => x.IsOverride).Count();
            dimensionInfo += $"number-overriden={numberOverriden} ";

            DimensionInfo = dimensionInfo;
        }
        else
        {
            DimensionInfo = string.Empty;
        }
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
                var item = _spriteItemVmFactory().Init(i);
                item.SpriteModified += OnSpriteModified;
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
                var item = _spriteItemVmFactory().Init(spriteFile);
                item.SpriteModified += OnSpriteModified;
                Items.Add(item);
                UpdateInfo(SelectedType);
            }
        }
    }

    private void OnSpriteModified(object? sender, SpriteFile file)
    {
        if (sender is not SpriteItemViewModel spriteItemViewModel)
        {
            return;
        }
        if (!File.Exists(file.File))
        {
            Items.Remove(spriteItemViewModel);
        }
        UpdateInfo(SelectedType);
    }

    public bool SetOverride(int id, string requestFileMsg)
    {
        return _spriteManager.SetOverride(SelectedType, id, requestFileMsg);
    }

    private void ExportAll()
    {
        var dir = _dialogService.ShowOpenFolderDialog(new OpenFolderDialogSettings
        {
            Title = "Select folder to export sprites into"
        });
        if (string.IsNullOrEmpty(dir))
        {
            return;
        }
        foreach (var spriteInfo in _spriteProvider.GetAllSpriteFiles(SelectedType))
        {
            string dest = FileUtil.MakeUniquePath(Path.Combine(dir, Path.GetFileName(spriteInfo.File)));
            File.Copy(spriteInfo.File, dest);
        }
        _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Export complete!", $"Sprites exported to: '{dir}'"));
    }
}
