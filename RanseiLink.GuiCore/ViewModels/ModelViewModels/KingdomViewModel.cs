#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class KingdomViewModel(IJumpService jumpService, IIdToNameService idToNameService, IAnimGuiManager animGuiManager) : ViewModelBase
{
    public void SetModel(KingdomId id, Kingdom model)
    {
        _id = id;
        _model = model;
        KingdomImageAnimVm = new(animGuiManager, AnimationTypeId.KuniImage2, (int)id);
        CastlemapAnimVm = new(animGuiManager, AnimationTypeId.Castlemap, (int)id);
        KingdomIconAnimVm = new(animGuiManager, AnimationTypeId.IconCastle, (int)id);
        RaiseAllPropertiesChanged();
    }

    public AnimationViewModel? KingdomImageAnimVm { get; private set; }
    public AnimationViewModel? CastlemapAnimVm { get; private set; }
    public AnimationViewModel? KingdomIconAnimVm { get; private set; }

    public ICommand JumpToBattleConfigCommand { get; } = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));

    public List<SelectorComboBoxItem> KingdomItems { get; } = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
}
