using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.GuiCore.Services;
using RanseiLink.GuiCore.Services.Concrete;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace RanseiLink.GuiCore.ViewModels;

public partial class GimmickObjectViewModel(IMapViewerService mapViewerService, INicknameService nicknameService, IMapManager mapManager, IOverrideDataProvider overrideDataProvider) : ViewModelBase
{
    public void SetModel(GimmickObjectId id, GimmickObject model)
    {
        _id = id;
        _model = model;
        ReloadVariants();
        RaiseAllPropertiesChanged();
    }

    public string Nickname
    {
        get => nicknameService.GetNickname(nameof(GimmickObjectId), (int)_id);
        set
        {
            if (Nickname != value)
            {
                nicknameService.SetNickname(nameof(GimmickObjectId), (int)_id, value);
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
            var dataFile = overrideDataProvider.GetDataFile(file);
            if (!File.Exists(dataFile.File))
            {
                break;
            }
            Variants.Add(new(mapViewerService, mapManager, _id, i));
        }
    }

    public ObservableCollection<GimmickObjectVariantVm> Variants { get; } = [];
}

public class GimmickObjectVariantVm : ViewModelBase
{
    private readonly IMapViewerService _mapViewerService;
    private readonly IMapManager _mapManager;
    private readonly GimmickObjectId _id;
    private readonly int _variant;

    public GimmickObjectVariantVm(IMapViewerService mapViewerService, IMapManager mapManager, GimmickObjectId id, int variant)
    {
        ExportObjCommand = new RelayCommand(Export);
        ImportTexturesCommand = new RelayCommand(ImportTextures);
        _mapViewerService = mapViewerService;
        _mapManager = mapManager;
        _id = id;
        _variant = variant;
        View3DModelCommand = new RelayCommand(View3DModel);
        RevertCommand = new RelayCommand(Revert, () => IsOverriden);
    }

    private async void Export()
    {
        await _mapManager.ExportObj(_id, _variant);
    }

    private async void ImportTextures()
    {
        await _mapManager.ImportObj_TexturesOnly(_id, _variant);
        RaisePropertyChanged(nameof(IsOverriden));
        RevertCommand.RaiseCanExecuteChanged();
    }

    private async void Revert()
    {
        await _mapManager.RevertModelToDefault(_id, _variant);
        RaisePropertyChanged(nameof(IsOverriden));
        RevertCommand.RaiseCanExecuteChanged();
    }

    public bool IsOverriden => _mapManager.IsOverriden(_id, _variant);
    public ICommand View3DModelCommand { get; }
    public ICommand ExportObjCommand { get; }
    public ICommand ImportTexturesCommand { get; }

    public RelayCommand RevertCommand { get; }

    public int Variant => _variant;

    public async void View3DModel()
    {
        await _mapViewerService.ShowDialog(_id, Variant);
    }
}
