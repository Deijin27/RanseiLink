#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.GuiCore.Services;

namespace RanseiLink.GuiCore.ViewModels;

public partial class KingdomViewModel : ViewModelBase, IBigViewModel
{
    private readonly IAnimGuiManager _animGuiManager;

    public KingdomViewModel(INicknameService nicknameService, IJumpService jumpService, IIdToNameService idToNameService, IAnimGuiManager animGuiManager)
    {
        _animGuiManager = animGuiManager;
        JumpToBattleConfigCommand = new RelayCommand<int>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, id));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonWorkspaceModule.Id, id));
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        BattleConfigItems = nicknameService.GetAllNicknames(nameof(BattleConfigId));
        PokemonItems = idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();
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

    public void SetModel(int id, object model)
    {
        SetModel((KingdomId)id, (Kingdom)model);
    }

    public AnimationViewModel? KingdomImageAnimVm { get; private set; }
    public AnimationViewModel? CastlemapAnimVm { get; private set; }
    public AnimationViewModel? KingdomIconAnimVm { get; private set; }

    public ICommand JumpToBattleConfigCommand { get; }
    public ICommand JumpToPokemonCommand { get; }
}
