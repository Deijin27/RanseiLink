#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Text;

namespace RanseiLink.GuiCore.ViewModels;

public partial class BaseWarriorViewModel : ViewModelBase, IBigViewModel
{
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly ICachedMsgBlockService _cachedMsgBlockService;
    private readonly IIdToNameService _idToNameService;
    private readonly IAsyncDialogService _dialogService;
    private WarriorNameTable _nameTable;
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    public BaseWarriorViewModel(CopyPasteViewModel copyPasteVm, IJumpService jumpService, IOverrideDataProvider overrideSpriteProvider, IIdToNameService idToNameService, 
        IBaseWarriorService baseWarriorService, ICachedMsgBlockService cachedMsgBlockService, SpriteItemViewModel.Factory spriteItemVmFactory, IAsyncDialogService dialogService)
    {
        _idToNameService = idToNameService;
        _dialogService = dialogService;
        _nameTable = baseWarriorService.NameTable;
        _spriteProvider = overrideSpriteProvider;
        _cachedMsgBlockService = cachedMsgBlockService;
        _spriteItemVmFactory = spriteItemVmFactory;

        JumpToWarriorSkillCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorSkillWorkspaceEditorModule.Id, id));
        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorWorkspaceModule.Id, id));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonWorkspaceModule.Id, id));
        JumpToWarriorNameCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorNameTableEditorModule.Id, id));
        JumpToSpeakerMessagesCommand = new RelayCommand(() =>
        {
            jumpService.JumpToMessageFilter($"{{{PnaConstNames.SpeakerId}:{SpeakerId}}}", false);
        });

        ViewSpritesCommand = new RelayCommand(ViewSprites);

        WarriorSkillItems = idToNameService.GetComboBoxItemsPlusDefault<IWarriorSkillService>();
        BaseWarriorItems = idToNameService.GetComboBoxItemsPlusDefault<IBaseWarriorService>();
        SpeakerItems = EnumUtil.GetValues<SpeakerId>().Select(x => new SelectorComboBoxItem((int)x, ((int)x).ToString())).ToList();

        this.PropertyChanged += BaseWarriorViewModel_PropertyChanged;
    }

    private void BaseWarriorViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Name):
                RaisePropertyChanged(nameof(WarriorNameValue));
                break;
            case nameof(Sprite):
                RaisePropertyChanged(nameof(SmallSpritePath));
                break;
            case nameof(RankUpCondition1):
                RaisePropertyChanged(nameof(RankUpCondition1_Quantity1_Name));
                RaisePropertyChanged(nameof(RankUpCondition1_Quantity2_Name));
                break;
            case nameof(RankUpCondition1_Quantity1):
                RaisePropertyChanged(nameof(RankUpCondition1_Quantity1_Name));
                break;
            case nameof(RankUpCondition1_Quantity2):
                RaisePropertyChanged(nameof(RankUpCondition1_Quantity2_Name));
                break;
            case nameof(RankUpCondition2):
                RaisePropertyChanged(nameof(RankUpCondition2_Quantity1_Name));
                RaisePropertyChanged(nameof(RankUpCondition2_Quantity2_Name));
                break;
            case nameof(RankUpCondition2_Quantity1):
                RaisePropertyChanged(nameof(RankUpCondition2_Quantity1_Name));
                break;
            case nameof(RankUpCondition2_Quantity2):
                RaisePropertyChanged(nameof(RankUpCondition2_Quantity2_Name));
                break;
        }
    }

    public void SetModel(WarriorId id, BaseWarrior model)
    {
        _id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public ICommand JumpToWarriorSkillCommand { get; }
    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToPokemonCommand { get; }
    public ICommand JumpToSpeakerMessagesCommand { get; }
    public ICommand JumpToWarriorNameCommand { get; }

    public string WarriorNameValue => _nameTable.GetEntry(Name);

    public string RankUpCondition1_Quantity1_Name => GetNameOfQuantityForRankUpCondition(RankUpCondition1, RankUpCondition1_Quantity1);

    public string RankUpCondition1_Quantity2_Name => GetNameOfQuantityForRankUpCondition(RankUpCondition1, RankUpCondition1_Quantity2);

    public string RankUpCondition2_Quantity1_Name => GetNameOfQuantityForRankUpCondition(RankUpCondition2, RankUpCondition2_Quantity1);

    public string RankUpCondition2_Quantity2_Name => GetNameOfQuantityForRankUpCondition(RankUpCondition2, RankUpCondition2_Quantity2);

    private string GetNameOfQuantityForRankUpCondition(RankUpConditionId id, int quantity)
    {
        if (quantity == 511)
        {
            return "Default";
        }
        switch (id)
        {
            case RankUpConditionId.AfterCompletingEpisode:
            case RankUpConditionId.DuringEpisode:
                return _cachedMsgBlockService.GetMsgOfType(MsgShortcut.EpisodeName, quantity);

            case RankUpConditionId.MonotypeGallery:
                return $"{(TypeId)quantity}";

            case RankUpConditionId.WarriorInSameArmyNotNearby:
            case RankUpConditionId.WarriorInSameKingdom:
                return $"{(WarriorLineId)quantity}";

            case RankUpConditionId.Pokemon:
                return _idToNameService.IdToName<IPokemonService>(quantity);

            default:
                return "";
        }
    }

    public string SmallSpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlBushouM, Sprite).File;

    public ICommand ViewSpritesCommand { get; }
    private async void ViewSprites()
    {
        List<SpriteFile> sprites = new();
        int id = Sprite;
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouS, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouB, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouCI, id));
        //sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouF, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouLL, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouM, id));
        //sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlBushouWu, id));

        var vm = new ImageListViewModel(sprites, _spriteItemVmFactory);
        await _dialogService.ShowDialog(vm);

        RaisePropertyChanged(nameof(SmallSpritePath));

    }

    public void SetModel(int id, object model)
    {
        SetModel((WarriorId)id, (BaseWarrior)model);
    }
}
