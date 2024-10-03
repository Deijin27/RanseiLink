#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Text;

namespace RanseiLink.GuiCore.ViewModels;

public partial class BaseWarriorViewModel : ViewModelBase
{
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly ICachedMsgBlockService _cachedMsgBlockService;
    private readonly IAsyncDialogService _dialogService;
    private WarriorNameTable _nameTable;
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    public BaseWarriorViewModel(CopyPasteViewModel copyPasteVm, IJumpService jumpService, IOverrideDataProvider overrideSpriteProvider, IIdToNameService idToNameService, 
        IBaseWarriorService baseWarriorService, ICachedMsgBlockService cachedMsgBlockService, SpriteItemViewModel.Factory spriteItemVmFactory, IAsyncDialogService dialogService)
    {
        _dialogService = dialogService;
        _nameTable = baseWarriorService.NameTable;
        _spriteProvider = overrideSpriteProvider;
        _cachedMsgBlockService = cachedMsgBlockService;
        _spriteItemVmFactory = spriteItemVmFactory;

        JumpToWarriorSkillCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorSkillSelectorEditorModule.Id, id));
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
        PokemonItems = idToNameService.GetComboBoxItemsPlusDefault<IPokemonService>();
        SpeakerItems = EnumUtil.GetValues<SpeakerId>().Select(x => new SelectorComboBoxItem((int)x, ((int)x).ToString())).ToList();
        CopyPasteVm = copyPasteVm;
        CopyPasteVm.ModelPasted += (_, __) => SetModel(_id, _model);

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
            case nameof(RankUpCondition2):
                RaisePropertyChanged(nameof(Quantity1ForRankUpConditionName));
                RaisePropertyChanged(nameof(Quantity2ForRankUpConditionName));
                break;
            case nameof(Quantity1ForRankUpCondition):
                RaisePropertyChanged(nameof(Quantity1ForRankUpConditionName));
                break;
            case nameof(Quantity2ForRankUpCondition):
                RaisePropertyChanged(nameof(Quantity2ForRankUpConditionName));
                break;
        }
    }

    public void SetModel(WarriorId id, BaseWarrior model)
    {
        _id = id;
        _model = model;
        CopyPasteVm.Model = model;
        RaiseAllPropertiesChanged();
    }

    public CopyPasteViewModel CopyPasteVm { get; }

    public List<SelectorComboBoxItem> WarriorSkillItems { get; }
    public List<SelectorComboBoxItem> BaseWarriorItems { get; }
    public List<SelectorComboBoxItem> PokemonItems { get; }
    public List<SelectorComboBoxItem> SpeakerItems { get; }

    public ICommand JumpToWarriorSkillCommand { get; }
    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToPokemonCommand { get; }
    public ICommand JumpToSpeakerMessagesCommand { get; }
    public ICommand JumpToWarriorNameCommand { get; }

    public string WarriorNameValue => _nameTable.GetEntry(Name);

    public string Quantity1ForRankUpConditionName => GetNameOfQuantityForRankUpCondition(RankUpCondition2, Quantity1ForRankUpCondition);

    public string Quantity2ForRankUpConditionName => GetNameOfQuantityForRankUpCondition(RankUpCondition2, Quantity2ForRankUpCondition);

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
}
