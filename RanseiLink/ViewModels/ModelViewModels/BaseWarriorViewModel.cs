using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Text;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public interface IBaseWarriorViewModel
{
    void SetModel(WarriorId id, BaseWarrior model);
}

public class BaseWarriorViewModel : ViewModelBase, IBaseWarriorViewModel
{
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly ICachedMsgBlockService _cachedMsgBlockService;
    private BaseWarrior _model;
    private WarriorNameTable _nameTable;
    private WarriorId _id;
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    public BaseWarriorViewModel(IJumpService jumpService, IOverrideDataProvider overrideSpriteProvider, IIdToNameService idToNameService, 
        IBaseWarriorService baseWarriorService, ICachedMsgBlockService cachedMsgBlockService, SpriteItemViewModel.Factory spriteItemVmFactory)
    {
        _model = new BaseWarrior();
        _nameTable = baseWarriorService.NameTable;
        _spriteProvider = overrideSpriteProvider;
        _cachedMsgBlockService = cachedMsgBlockService;
        _spriteItemVmFactory = spriteItemVmFactory;

        JumpToWarriorSkillCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorSkillSelectorEditorModule.Id, id));
        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(BaseWarriorSelectorEditorModule.Id, id));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonSelectorEditorModule.Id, id));
        JumpToSpeakerMessagesCommand = new RelayCommand(() =>
        {
            jumpService.JumpToMessageFilter($"{{{PnaConstNames.SpeakerId}:{SpeakerId}}}", false);
        });

        ViewSpritesCommand = new RelayCommand(ViewSprites);

        WarriorSkillItems = idToNameService.GetComboBoxItemsPlusDefault<IWarriorSkillService>();
        BaseWarriorItems = idToNameService.GetComboBoxItemsPlusDefault<IBaseWarriorService>();
        PokemonItems = idToNameService.GetComboBoxItemsPlusDefault<IPokemonService>();
        SpeakerItems = EnumUtil.GetValues<SpeakerId>().Select(x => new SelectorComboBoxItem((int)x, ((int)x).ToString())).ToList();
    }

    public void SetModel(WarriorId id, BaseWarrior model)
    {
        _id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public List<SelectorComboBoxItem> WarriorSkillItems { get; }
    public List<SelectorComboBoxItem> BaseWarriorItems { get; }
    public List<SelectorComboBoxItem> PokemonItems { get; }
    public List<SelectorComboBoxItem> SpeakerItems { get; }

    public ICommand JumpToWarriorSkillCommand { get; }
    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToPokemonCommand { get; }
    public ICommand JumpToSpeakerMessagesCommand { get; }

    public int SpeakerId
    {
        get => (int)_model.SpeakerId;
        set => RaiseAndSetIfChanged(_model.SpeakerId, (SpeakerId)value, v => _model.SpeakerId = v);
    }

    public GenderId Gender
    {
        get => _model.Gender;
        set => RaiseAndSetIfChanged(_model.Gender, value, v => _model.Gender = v);
    }

    public int WarriorName
    {
        get => _model.WarriorName;
        set
        {
            if (RaiseAndSetIfChanged(_model.WarriorName, value, v => _model.WarriorName = v))
            {
                RaisePropertyChanged(nameof(WarriorNameValue));
            }
        }
    }

    public string WarriorNameValue => _nameTable.GetEntry(WarriorName);

    public TypeId Speciality1
    {
        get => _model.Speciality1;
        set => RaiseAndSetIfChanged(_model.Speciality1, value, v => _model.Speciality1 = v);
    }

    public TypeId Speciality2
    {
        get => _model.Speciality2;
        set => RaiseAndSetIfChanged(_model.Speciality2, value, v => _model.Speciality2 = v);
    }

    public TypeId Weakness1
    {
        get => _model.Weakness1;
        set => RaiseAndSetIfChanged(_model.Weakness1, value, v => _model.Weakness1 = v);
    }

    public TypeId Weakness2
    {
        get => _model.Weakness2;
        set => RaiseAndSetIfChanged(_model.Weakness2, value, v => _model.Weakness2 = v);
    }

    public int Skill
    {
        get => (int)_model.Skill;
        set => RaiseAndSetIfChanged(_model.Skill, (WarriorSkillId)value, v => _model.Skill = v);
    }

    public int RankUp
    {
        get => (int)_model.RankUp;
        set => RaiseAndSetIfChanged(_model.RankUp, (WarriorId)value, v => _model.RankUp = v);
    }

    public int Power
    {
        get => _model.Power;
        set => RaiseAndSetIfChanged(_model.Power, value, v => _model.Power = v);
    }

    public int Wisdom
    {
        get => _model.Wisdom;
        set => RaiseAndSetIfChanged(_model.Wisdom, value, v => _model.Wisdom = v);
    }

    public int Charisma
    {
        get => _model.Charisma;
        set => RaiseAndSetIfChanged(_model.Charisma, value, v => _model.Charisma = v);
    }

    public int Capacity
    {
        get => _model.Capacity;
        set => RaiseAndSetIfChanged(_model.Capacity, value, v => _model.Capacity = v);
    }

    public int RankUpPokemon1
    {
        get => (int)_model.RankUpPokemon1;
        set => RaiseAndSetIfChanged(_model.RankUpPokemon1, (PokemonId)value, v => _model.RankUpPokemon1 = v);
    }

    public int RankUpPokemon2
    {
        get => (int)_model.RankUpPokemon2;
        set => RaiseAndSetIfChanged(_model.RankUpPokemon2, (PokemonId)value, v => _model.RankUpPokemon2 = v);
    }

    public int RankUpLink
    {
        get => _model.RankUpLink;
        set => RaiseAndSetIfChanged(_model.RankUpLink, value, v => _model.RankUpLink = value);
    }

    public RankUpConditionId RankUpCondition1
    {
        get => _model.RankUpCondition1;
        set => RaiseAndSetIfChanged(_model.RankUpCondition1, value, v => _model.RankUpCondition1 = value);
    }

    public RankUpConditionId RankUpCondition2
    {
        get => _model.RankUpCondition2;
        set
        {
            if (RaiseAndSetIfChanged(_model.RankUpCondition2, value, v => _model.RankUpCondition2 = value))
            {
                RaisePropertyChanged(nameof(Quantity1ForRankUpConditionName));
                RaisePropertyChanged(nameof(Quantity2ForRankUpConditionName));
            }
        }
    }

    public int Quantity1ForRankUpCondition
    {
        get => _model.Quantity1ForRankUpCondition;
        set 
        { 
            if (RaiseAndSetIfChanged(_model.Quantity1ForRankUpCondition, value, v => _model.Quantity1ForRankUpCondition = value)) 
            {
                RaisePropertyChanged(nameof(Quantity1ForRankUpConditionName));
            } 
        }
    }

    public int Quantity2ForRankUpCondition
    {
        get => _model.Quantity2ForRankUpCondition;
        set
        {
            if (RaiseAndSetIfChanged(_model.Quantity2ForRankUpCondition, value, v => _model.Quantity2ForRankUpCondition = value))
            {
                RaisePropertyChanged(nameof(Quantity2ForRankUpConditionName));
            }
        }
    }

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

    public int Sprite
    {
        get => _model.Sprite;
        set
        { 
            if (RaiseAndSetIfChanged(_model.Sprite, value, v => _model.Sprite = v))
            {
                RaisePropertyChanged(nameof(SmallSpritePath));
            }
        }
    }

    public string SmallSpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlBushouM, Sprite).File;

    public ICommand ViewSpritesCommand { get; }
    private void ViewSprites()
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

        var dialog = new Dialogs.ImageListDialog(sprites, _spriteItemVmFactory) { Owner = System.Windows.Application.Current.MainWindow };
        dialog.ShowDialog();

        RaisePropertyChanged(nameof(SmallSpritePath));

    }
}
