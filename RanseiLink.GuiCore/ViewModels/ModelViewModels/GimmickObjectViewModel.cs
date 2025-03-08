using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public partial class GimmickObjectViewModel(INicknameService nicknameService, IMapManager mapManager, IOverrideDataProvider overrideDataProvider) : ViewModelBase
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
            Variants.Add(new(mapManager, _id, i));
        }
    }

    public ObservableCollection<GimmickObjectVariantVm> Variants { get; } = [];
}

public class GimmickObjectVariantVm : ViewModelBase
{
    public GimmickObjectVariantVm(IMapManager mapManager, GimmickObjectId id, int variant)
    {
        ExportObjCommand = new RelayCommand(async () => await mapManager.ExportObj(id, variant));
        Variant = variant;
    }
    public ICommand ExportObjCommand { get; }
    public int Variant { get; }
}


