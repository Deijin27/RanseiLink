#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class KingdomViewModel : ViewModelBase
{
    private readonly IAnimGuiManager _animGuiManager;

    public KingdomViewModel(IJumpService jumpService, IIdToNameService idToNameService, IAnimGuiManager animGuiManager)
    {
        _animGuiManager = animGuiManager;
        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
    }

    public void SetModel(KingdomId id, Kingdom model)
    {
        _id = id;
        _model = model;
        KingdomImageAnimVm = new(_animGuiManager, AnimationTypeId.KuniImage2, (int)id);
        CastlemapAnimVm = new(_animGuiManager, AnimationTypeId.Castlemap, (int)id);
        KingdomIconAnimVm = new(_animGuiManager, AnimationTypeId.IconCastle, (int)id);
        RaiseAllPropertiesChanged();
    }

    public AnimationViewModel? KingdomImageAnimVm { get; private set; }
    public AnimationViewModel? CastlemapAnimVm { get; private set; }
    public AnimationViewModel? KingdomIconAnimVm { get; private set; }

    public ICommand JumpToBattleConfigCommand { get; }
}
