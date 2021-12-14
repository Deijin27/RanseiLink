using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorGridSelectorViewModel : ViewModelBase, ISaveableRefreshable
{
    private readonly IDataService _dataService;
    public ScenarioWarriorGridSelectorViewModel(IServiceContainer container, IEditorContext context)
    {
        _dataService = context.DataService;
    }

    private ScenarioId _selectedScenario = ScenarioId.TheLegendOfRansei;
    public ScenarioId SelectedScenario
    {
        get => _selectedScenario;
        set
        {
            if (_selectedScenario != value)
            {
                Save();
                _selectedScenario = value;
                Refresh();
            }
        }
    }

    private readonly List<IScenarioWarrior> _scenarioWarriors = new();
    public ObservableCollection<ScenarioWarriorGridItemViewModel> Items { get; } = new();

    public void Refresh()
    {
        Items.Clear();
        _scenarioWarriors.Clear();
        using var scenarioWarriorService = _dataService.ScenarioWarrior.Disposable();
        using var scenarioPokemonService = _dataService.ScenarioPokemon.Disposable();

        for (int i = 0; i < 200; i++)
        {
            var scenarioWarrior = scenarioWarriorService.Retrieve(_selectedScenario, i);
            PokemonId initPokemon;
            AbilityId initAbility;
            int initScenarioPokemon;
            
            if (scenarioWarrior.ScenarioPokemonIsDefault)
            {
                initPokemon = PokemonId.Default;
                initAbility = AbilityId.NoAbility;
                initScenarioPokemon = -1;
            }
            else
            {
                var scenarioPokemon = scenarioPokemonService.Retrieve(_selectedScenario, (int)scenarioWarrior.ScenarioPokemon);
                initPokemon = scenarioPokemon.Pokemon;
                initAbility = scenarioPokemon.Ability;
                initScenarioPokemon = (int)scenarioWarrior.ScenarioPokemon;
            }

            _scenarioWarriors.Add(scenarioWarrior);
            Items.Add(new ScenarioWarriorGridItemViewModel(i, scenarioWarrior)
            {
                Pokemon = initPokemon,
                ScenarioPokemonId = initScenarioPokemon,
                PokemonAbility = initAbility
            });
        }
    }

    public void Save()
    {
        using var scenarioWarriorService = _dataService.ScenarioWarrior.Disposable();
        using var scenarioPokemonService = _dataService.ScenarioPokemon.Disposable();

        for (int i = 0; i < 200; i++)
        {
            var warrior = _scenarioWarriors[i];
            var vm = Items[i];
            if (vm.Pokemon == PokemonId.Default || vm.ScenarioPokemonId == -1)
            {
                warrior.ScenarioPokemonIsDefault = true;
            }
            else
            {
                warrior.ScenarioPokemonIsDefault = false;
                warrior.ScenarioPokemon = (uint)vm.ScenarioPokemonId;
                var scenarioPokemon = scenarioPokemonService.Retrieve(_selectedScenario, vm.ScenarioPokemonId);
                scenarioPokemon.Ability = vm.PokemonAbility;
                scenarioPokemon.Pokemon = vm.Pokemon;
                scenarioPokemonService.Save(_selectedScenario, vm.ScenarioPokemonId, scenarioPokemon);
            }
            scenarioWarriorService.Save(_selectedScenario, i, warrior);
        }
    }
}
