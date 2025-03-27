using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.GuiCore.Services;
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
    private readonly GimmickObjectId _id;

    public GimmickObjectVariantVm(IMapViewerService mapViewerService, IMapManager mapManager, GimmickObjectId id, int variant)
    {
        ExportObjCommand = new RelayCommand(async () => await mapManager.ExportObj(id, variant));
        _mapViewerService = mapViewerService;
        _id = id;
        Variant = variant;
        View3DModelCommand = new RelayCommand(View3DModel);
    }
    public ICommand View3DModelCommand { get; }
    public ICommand ExportObjCommand { get; }
    public int Variant { get; }

    public async void View3DModel()
    {
        await _mapViewerService.ShowDialog(_id, Variant);
    }
}
