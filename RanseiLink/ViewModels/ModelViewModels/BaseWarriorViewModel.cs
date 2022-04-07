using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public interface IBaseWarriorViewModel
{
    void SetModel(BaseWarrior model);
}

public class BaseWarriorViewModel : ViewModelBase, IBaseWarriorViewModel
{
    private readonly IOverrideSpriteProvider _spriteProvider;
    private BaseWarrior _model;
    private WarriorNameTable _nameTable;
    public BaseWarriorViewModel(IJumpService jumpService, IOverrideSpriteProvider overrideSpriteProvider, IIdToNameService idToNameService, IBaseWarriorService baseWarriorService)
    {
        _model = new BaseWarrior();
        _nameTable = baseWarriorService.NameTable;
        _spriteProvider = overrideSpriteProvider;

        JumpToWarriorSkillCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorSkillSelectorEditorModule.Id, id));
        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(BaseWarriorSelectorEditorModule.Id, id));
        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonSelectorEditorModule.Id, id));

        WarriorSkillItems = idToNameService.GetComboBoxItemsPlusDefault<IWarriorSkillService>();
        BaseWarriorItems = idToNameService.GetComboBoxItemsPlusDefault<IBaseWarriorService>();
        PokemonItems = idToNameService.GetComboBoxItemsPlusDefault<IPokemonService>();
    }

    public void SetModel(BaseWarrior model)
    {
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public List<SelectorComboBoxItem> WarriorSkillItems { get; }
    public List<SelectorComboBoxItem> BaseWarriorItems { get; }
    public List<SelectorComboBoxItem> PokemonItems { get; }

    public ICommand JumpToWarriorSkillCommand { get; }
    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToPokemonCommand { get; }


    public WarriorSprite2Id Sprite_Unknown
    {
        get => _model.Sprite_Unknown;
        set => RaiseAndSetIfChanged(_model.Sprite_Unknown, value, v => _model.Sprite_Unknown = v);
    }

    public GenderId Gender
    {
        get => _model.Gender;
        set => RaiseAndSetIfChanged(_model.Gender, value, v => _model.Gender = v);
    }

    public uint WarriorName
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

    public uint Power
    {
        get => _model.Power;
        set => RaiseAndSetIfChanged(_model.Power, value, v => _model.Power = v);
    }

    public uint Wisdom
    {
        get => _model.Wisdom;
        set => RaiseAndSetIfChanged(_model.Wisdom, value, v => _model.Wisdom = v);
    }

    public uint Charisma
    {
        get => _model.Charisma;
        set => RaiseAndSetIfChanged(_model.Charisma, value, v => _model.Charisma = v);
    }

    public uint Capacity
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

    public uint RankUpLink
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
        set => RaiseAndSetIfChanged(_model.RankUpCondition2, value, v => _model.RankUpCondition2 = value);
    }

    public uint Quantity1ForRankUpCondition
    {
        get => _model.Quantity1ForRankUpCondition;
        set => RaiseAndSetIfChanged(_model.Quantity1ForRankUpCondition, value, v => _model.Quantity1ForRankUpCondition = value);
    }

    public uint Quantity2ForRankUpCondition
    {
        get => _model.Quantity2ForRankUpCondition;
        set => RaiseAndSetIfChanged(_model.Quantity2ForRankUpCondition, value, v => _model.Quantity2ForRankUpCondition = value);
    }

    public uint Sprite
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
}
