using RanseiLink.Core;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class SpriteTypeViewModel : ViewModelBase
{
    private readonly SpriteItemViewModelFactory _spriteItemVmFactory;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IDialogService _dialogService;
    private readonly ISpriteManager _spriteManager;
    private string _dimensionInfo;
    private bool _canAddNew;

    public SpriteTypeViewModel(ISpriteManager spriteManager, IOverrideDataProvider overrideSpriteProvider, IDialogService dialogService, SpriteItemViewModelFactory spriteItemVmFactory)
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
        IGraphicsInfo info = GraphicsInfoResource.Get(type);

        _canAddNew = !info.FixedAmount;

        if (info is MiscConstants)
        {
            DimensionInfo = "";
            return;
        }

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
                var item = _spriteItemVmFactory(i);
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
                var item = _spriteItemVmFactory(spriteFile);
                item.SpriteModified += OnSpriteModified;
                Items.Add(item);
                UpdateInfo(SelectedType);
            }
        }
    }

    private void OnSpriteModified(object sender, SpriteFile file)
    {
        if (!File.Exists(file.File))
        {
            Items.Remove((SpriteItemViewModel)sender);
        }
        UpdateInfo(SelectedType);
    }

    public bool SetOverride(int id, string requestFileMsg)
    {
        return _spriteManager.SetOverride(SelectedType, id, requestFileMsg);
    }

    private void ExportAll()
    {
        if (!_dialogService.RequestFolder("Select folder to export sprites into", out string dir))
        {
            return;
        }
        foreach (var spriteInfo in _spriteProvider.GetAllSpriteFiles(SelectedType))
        {
            string dest = FileUtil.MakeUniquePath(Path.Combine(dir, Path.GetFileName(spriteInfo.File)));
            File.Copy(spriteInfo.File, dest);
        }
        _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Export complete!", $"Sprites exported to: '{dir}'"));
    }
}
