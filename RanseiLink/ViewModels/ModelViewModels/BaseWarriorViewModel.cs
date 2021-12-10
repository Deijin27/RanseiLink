using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate BaseWarriorViewModel BaseWarriorViewModelFactory(IBaseWarrior model, IEditorContext context);

public class BaseWarriorViewModel : ViewModelBase
{
    private readonly IBaseWarrior _model;

    public BaseWarriorViewModel(IBaseWarrior model, IEditorContext context)
    {
        _model = model;
        var jumpService = context.JumpService;

        JumpToWarriorSkillCommand = new RelayCommand<WarriorSkillId>(jumpService.JumpToWarriorSkill);
        JumpToBaseWarriorCommand = new RelayCommand<WarriorId>(jumpService.JumpToBaseWarrior);
        JumpToPokemonCommand = new RelayCommand<PokemonId>(jumpService.JumpToPokemon);
    }

    public ICommand JumpToWarriorSkillCommand { get; }
    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToPokemonCommand { get; }
    
    public WarriorSpriteId[] SpriteItems { get; } = EnumUtil.GetValues<WarriorSpriteId>().ToArray();

    public WarriorSpriteId Sprite
    {
        get => _model.Sprite;
        set => RaiseAndSetIfChanged(_model.Sprite, value, v => _model.Sprite = v);
    }
    public WarriorSprite2Id[] Sprite2Items { get; } = EnumUtil.GetValues<WarriorSprite2Id>().ToArray();

    public WarriorSprite2Id Sprite_Unknown
    {
        get => _model.Sprite_Unknown;
        set => RaiseAndSetIfChanged(_model.Sprite_Unknown, value, v => _model.Sprite_Unknown = v);
    }

    public GenderId[] GenderItems { get; } = EnumUtil.GetValues<GenderId>().ToArray();

    public GenderId Gender
    {
        get => _model.Gender;
        set => RaiseAndSetIfChanged(_model.Gender, value, v => _model.Gender = v);
    }

    public uint WarriorName
    {
        get => _model.WarriorName;
        set => RaiseAndSetIfChanged(_model.WarriorName, value, v => _model.WarriorName = v);
    }

    public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();

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

    public WarriorSkillId[] WarriorSkillItems { get; } = EnumUtil.GetValues<WarriorSkillId>().ToArray();

    public WarriorSkillId Skill
    {
        get => _model.Skill;
        set => RaiseAndSetIfChanged(_model.Skill, value, v => _model.Skill = v);
    }

    public WarriorId[] WarriorItems { get; } = EnumUtil.GetValues<WarriorId>().ToArray();

    public WarriorId RankUp
    {
        get => _model.RankUp;
        set => RaiseAndSetIfChanged(_model.RankUp, value, v => _model.RankUp = v);
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

    public PokemonId[] PokemonItems { get; } = EnumUtil.GetValues<PokemonId>().ToArray();

    public PokemonId RankUpPokemon1
    {
        get => _model.RankUpPokemon1;
        set => RaiseAndSetIfChanged(_model.RankUpPokemon1, value, v => _model.RankUpPokemon1 = value);
    }

    public PokemonId RankUpPokemon2
    {
        get => _model.RankUpPokemon2;
        set => RaiseAndSetIfChanged(_model.RankUpPokemon2, value, v => _model.RankUpPokemon2 = value);
    }

    public uint RankUpLink
    {
        get => _model.RankUpLink;
        set => RaiseAndSetIfChanged(_model.RankUpLink, value, v => _model.RankUpLink = value);
    }

    public RankUpConditionId[] RankUpConditionItems { get; } = EnumUtil.GetValues<RankUpConditionId>().ToArray();

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
}
