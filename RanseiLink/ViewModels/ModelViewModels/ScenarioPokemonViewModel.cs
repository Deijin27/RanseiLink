using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate ScenarioPokemonViewModel ScenarioPokemonViewModelFactory(IScenarioPokemon model, IEditorContext context);

public class ScenarioPokemonViewModel : ViewModelBase
{
    private readonly IScenarioPokemon _model;

    public ScenarioPokemonViewModel(IScenarioPokemon model, IEditorContext context)
    {
        _model = model;

        var jumpService = context.JumpService;
        JumpToPokemonCommand = new RelayCommand<PokemonId>(jumpService.JumpToPokemon);
        JumpToAbilityCommand = new RelayCommand<AbilityId>(jumpService.JumpToAbility);
    }

    public ICommand JumpToPokemonCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public PokemonId[] PokemonItems { get; } = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();
    public AbilityId[] AbilityItems { get; } = EnumUtil.GetValues<AbilityId>().ToArray();

    public PokemonId Pokemon
    {
        get => _model.Pokemon;
        set => RaiseAndSetIfChanged(_model.Pokemon, value, v => _model.Pokemon = v);
    }

    public AbilityId Ability
    {
        get => _model.Ability;
        set => RaiseAndSetIfChanged(_model.Ability, value, v => _model.Ability = v);
    }

    public uint HpIv
    {
        get => _model.HpIv;
        set => RaiseAndSetIfChanged(_model.HpIv, value, v => _model.HpIv = v);
    }

    public uint AtkIv
    {
        get => _model.AtkIv;
        set => RaiseAndSetIfChanged(_model.AtkIv, value, v => _model.AtkIv = v);
    }

    public uint DefIv
    {
        get => _model.DefIv;
        set => RaiseAndSetIfChanged(_model.DefIv, value, v => _model.DefIv = v);
    }

    public uint SpeIv
    {
        get => _model.SpeIv;
        set => RaiseAndSetIfChanged(_model.SpeIv, value, v => _model.SpeIv = v);
    }
}
