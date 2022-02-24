using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorGridSelectorViewModel : ViewModelBase, ISaveableRefreshable, IGridViewModel<ScenarioWarriorGridItemViewModel>
{
    private readonly IModServiceContainer _dataService;
    private readonly IDialogService _dialogService;
    public ScenarioWarriorGridSelectorViewModel(IServiceContainer container, IEditorContext context)
    {
        _dataService = context.DataService;
        _dialogService = container.Resolve<IDialogService>();
    }

    private ScenarioId _selectedScenario = ScenarioId.TheLegendOfRansei;
    public ScenarioId SelectedScenario
    {
        get => _selectedScenario;
        set
        {
            if (_selectedScenario != value)
            {
                if (CanSave())
                {
                    Save();
                    _selectedScenario = value;
                    Refresh();
                }
                
            }
        }
    }

    private readonly List<IScenarioWarrior> _scenarioWarriors = new();
    public ObservableCollection<ScenarioWarriorGridItemViewModel> Items { get; } = new();

    public int FrozenColumnCount => 1;

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
            
            if (scenarioWarrior.ScenarioPokemonIsDefault(0))
            {
                initPokemon = PokemonId.Default;
                initAbility = AbilityId.NoAbility;
                initScenarioPokemon = DefaultScenarioPokemonId;
            }
            else
            {
                var scenarioPokemon = scenarioPokemonService.Retrieve(_selectedScenario, scenarioWarrior.GetScenarioPokemon(0));
                initPokemon = scenarioPokemon.Pokemon;
                initAbility = scenarioPokemon.Ability;
                initScenarioPokemon = scenarioWarrior.GetScenarioPokemon(0);
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

    private const int DefaultScenarioPokemonId = -1;

    public bool CanSave()
    {
        bool problemsExist = false;
        var sb = new StringBuilder();
        // find scenario warriors with the same scenario pokemon and validate that all properties are the same for them
        foreach (var dupes in Items.GroupBy(i => i.ScenarioPokemonId).Where(i => i.Key != DefaultScenarioPokemonId && i.Count() > 1).Select(i => i.ToArray()))
        {
            var first = dupes.First();
            if (!dupes.All(i => i.Pokemon == first.Pokemon && i.PokemonAbility == first.PokemonAbility))
            {
                problemsExist = true;
                sb.AppendLine($"More than one scenario warrior has the same scenario pokemon '{first.ScenarioPokemonId}' but the pokemon and ability associated with them are different.\n");
            }
        }

        if (!problemsExist)
        {
            return true;
        }

        var result = _dialogService.ShowMessageBox(new MessageBoxArgs(
            "Problems with scenario pokemon",
            "There are some issues with the scenario warriors' scenario pokemon. If you ignore these then the final data written to the scenario pokemon will be the last data encountered in the table. The issues:\n\n"
            + sb.ToString(),
            new[] { new MessageBoxButton("Ignore and proceed", MessageBoxResult.Ok), new MessageBoxButton("Cancel", MessageBoxResult.Cancel) }
            ));

        return result == MessageBoxResult.Ok;
    }

    public void Save()
    {
        using var scenarioWarriorService = _dataService.ScenarioWarrior.Disposable();
        using var scenarioPokemonService = _dataService.ScenarioPokemon.Disposable();

        for (int i = 0; i < 200; i++)
        {
            var warrior = _scenarioWarriors[i];
            var vm = Items[i];
            if (vm.Pokemon == PokemonId.Default || vm.ScenarioPokemonId == DefaultScenarioPokemonId)
            {
                warrior.MakeScenarioPokemonDefault(0);
            }
            else
            {
                warrior.SetScenarioPokemon(0, (ushort)vm.ScenarioPokemonId);
                var scenarioPokemon = scenarioPokemonService.Retrieve(_selectedScenario, vm.ScenarioPokemonId);
                scenarioPokemon.Ability = vm.PokemonAbility;
                scenarioPokemon.Pokemon = vm.Pokemon;
                scenarioPokemonService.Save(_selectedScenario, vm.ScenarioPokemonId, scenarioPokemon);
            }
            scenarioWarriorService.Save(_selectedScenario, i, warrior);
        }
    }
}
