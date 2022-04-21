using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public interface IScenarioPokemonViewModel
{
    void SetModel(ScenarioId scenario, int id, ScenarioPokemon model);
}

public class ScenarioPokemonViewModel : ViewModelBase, IScenarioPokemonViewModel
{
    private ScenarioPokemon _model;
    private int _id;
    private ScenarioId _scenario;

    public ScenarioPokemonViewModel(
        IJumpService jumpService, 
        IScenarioWarriorService scenarioWarriorService, 
        IIdToNameService idToNameService)
    {
        _model = new ScenarioPokemon(); ;

        JumpToPokemonCommand = new RelayCommand<int>(id => jumpService.JumpTo(PokemonSelectorEditorModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilitySelectorEditorModule.Id, id));
        JumpToFirstWarriorCommand = new RelayCommand(() =>
        {
            int i = 0;
            foreach (var sw in scenarioWarriorService.Retrieve((int)_scenario).Enumerate())
            {
                if (!sw.ScenarioPokemonIsDefault(0) && sw.GetScenarioPokemon(0) == _id)
                {
                    jumpService.JumpToNested(ScenarioWarriorSelectorEditorModule.Id, (int)_scenario, i);
                    return;
                }
                i++;
            }
        });

        PokemonItems = idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();
        PokemonItems.Add(new SelectorComboBoxItem(511, "Default"));
        AbilityItems = idToNameService.GetComboBoxItemsPlusDefault<IAbilityService>();
    }

    public void SetModel(ScenarioId scenario, int id, ScenarioPokemon model)
    {
        _model = model;
        _scenario = scenario;
        _id = id;
        RaiseAllPropertiesChanged();
    }

    public ICommand JumpToPokemonCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    /// <summary>
    /// Jump to first scenario warrior with this scenario pokemon
    /// </summary>
    public ICommand JumpToFirstWarriorCommand { get; }

    public List<SelectorComboBoxItem> PokemonItems { get; }
    public List<SelectorComboBoxItem> AbilityItems { get; }

    public int Pokemon
    {
        get => (int)_model.Pokemon;
        set => RaiseAndSetIfChanged(_model.Pokemon, (PokemonId)value, v => _model.Pokemon = v);
    }

    public int Ability
    {
        get => (int)_model.Ability;
        set => RaiseAndSetIfChanged(_model.Ability, (AbilityId)value, v => _model.Ability = v);
    }

    public int HpIv
    {
        get => _model.HpIv;
        set => RaiseAndSetIfChanged(_model.HpIv, value, v => _model.HpIv = v);
    }

    public int AtkIv
    {
        get => _model.AtkIv;
        set => RaiseAndSetIfChanged(_model.AtkIv, value, v => _model.AtkIv = v);
    }

    public int DefIv
    {
        get => _model.DefIv;
        set => RaiseAndSetIfChanged(_model.DefIv, value, v => _model.DefIv = v);
    }

    public int SpeIv
    {
        get => _model.SpeIv;
        set => RaiseAndSetIfChanged(_model.SpeIv, value, v => _model.SpeIv = v);
    }

    public int Exp
    {
        get => _model.Exp;
        set
        {
            if (RaiseAndSetIfChanged(_model.Exp, value, v => _model.Exp = (ushort)v))
            {
                RaisePropertyChanged(nameof(ApproximateLink));
            }
        }
    }

    public double ApproximateLink => Math.Round(LinkCalculator.CalculateLink(_model.Exp));
}
