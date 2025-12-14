using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public partial class GimmickObjectViewModel : ViewModelBase
{
    private readonly IMapViewerService _mapViewerService;
    private readonly INicknameService _nicknameService;
    private readonly IMapManager _mapManager;
    private readonly IOverrideDataProvider _overrideDataProvider;

    public GimmickObjectViewModel(IMapViewerService mapViewerService, INicknameService nicknameService, IMapManager mapManager, IOverrideDataProvider overrideDataProvider)
    {
        _mapViewerService = mapViewerService;
        _nicknameService = nicknameService;
        _mapManager = mapManager;
        _overrideDataProvider = overrideDataProvider;


        ExportObjCommand = new RelayCommand(Export);
        ImportTexturesCommand = new RelayCommand(ImportTextures);
        RevertCommand = new RelayCommand(Revert, () => IsOverriden);
    }

    public void SetModel(GimmickObjectId id, GimmickObject model)
    {
        _id = id;
        _model = model;
        ReloadVariants();
        RaiseAllPropertiesChanged();
        RevertCommand.RaiseCanExecuteChanged();
    }

    public string Nickname
    {
        get => _nicknameService.GetNickname(nameof(GimmickObjectId), (int)_id);
        set
        {
            if (Nickname != value)
            {
                _nicknameService.SetNickname(nameof(GimmickObjectId), (int)_id, value);
                RaisePropertyChanged();
            }
        }
    }

    private void ReloadVariants()
    {
        Variants.Clear();
        for (int i = 0; i < 10; i++)
        {
            var file = Core.Services.Constants.ResolveGimmickModelFilePath(_id, i);
            var dataFile = _overrideDataProvider.GetDataFile(file);
            if (!File.Exists(dataFile.File))
            {
                break;
            }
            Variants.Add(new(this, i));
        }
    }

    public ObservableCollection<GimmickObjectVariantVm> Variants { get; } = [];

    private async void Export()
    {
        await _mapManager.ExportObj(_id, Variants.Select(x => x.Variant).ToArray());
    }

    private async void ImportTextures()
    {
        await _mapManager.ImportObj_TexturesOnly(_id, Variants.Select(x => x.Variant).ToArray());
        RaisePropertyChanged(nameof(IsOverriden));
        RevertCommand.RaiseCanExecuteChanged();
    }

    private async void Revert()
    {
        await _mapManager.RevertModelToDefault(_id, Variants.Select(x => x.Variant).ToArray());
        RaisePropertyChanged(nameof(IsOverriden));
        RevertCommand.RaiseCanExecuteChanged();
    }

    public bool IsOverriden => _mapManager.IsOverriden(_id, Variants.Select(x => x.Variant).ToArray());

    public ICommand ExportObjCommand { get; }
    public ICommand ImportTexturesCommand { get; }

    public RelayCommand RevertCommand { get; }

    public Task View3DModel(int variant)
    {
        return _mapViewerService.ShowDialog(_id, variant);
    }
}

public class GimmickObjectVariantVm : ViewModelBase
{
    private readonly GimmickObjectViewModel _parent;
    private readonly int _variant;

    public GimmickObjectVariantVm(GimmickObjectViewModel parent, int variant)
    {
        _parent = parent;
        _variant = variant;
        View3DModelCommand = new RelayCommand(View3DModel);
        
    }

    private async void View3DModel()
    {
        await _parent.View3DModel(_variant);
    }
    

    public ICommand View3DModelCommand { get; }
    public int Variant => _variant;

    
}
