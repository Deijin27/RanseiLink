using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public record ListItem(string ItemName, ISaveableRefreshable ItemValue);

public class MainEditorViewModel : ViewModelBase, ISaveable
{
    private readonly IServiceContainer _container;
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly IModService _modService;

    public ICommand CommitRomCommand { get; }

    private ISaveableRefreshable _currentVm;
    public ISaveableRefreshable CurrentVm
    {
        get => _currentVm;
        set
        {
            if (_currentVm != value)
            {
                _currentVm?.Save();
                _currentVm = value;
                _currentVm?.Refresh();
                RaisePropertyChanged();
            }
        }
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }

    public ListItem CurrentListItem { get; set; } // bound one way to source

    public ModInfo Mod { get; }

    private IList<ListItem> _listItems;
    public IList<ListItem> ListItems
    {
        get => _listItems;
        set => RaiseAndSetIfChanged(ref _listItems, value);
    }

    public MainEditorViewModel(IServiceContainer container, ModInfo mod)
    {
        _container = container;
        var dataServiceFactory = container.Resolve<DataServiceFactory>();
        _dialogService = container.Resolve<IDialogService>();
        _modService = container.Resolve<IModService>();

        PluginItems = container.Resolve<IPluginLoader>().LoadPlugins(out var _);

        Mod = mod;
        _dataService = dataServiceFactory(Mod);

        ReloadListItems();

        CurrentVm = ListItems[0].ItemValue;
        CommitRomCommand = new RelayCommand(CommitRom);
    }

    private void ReloadListItems()
    {
        ListItems = new List<ListItem>()
        {
            new ListItem("Pokemon", new PokemonSelectorViewModel(_dialogService, PokemonId.Eevee, _dataService.Pokemon)),
            new ListItem("Moves", new MoveSelectorViewModel(_dialogService, MoveId.Splash, _dataService.Move)),
            new ListItem("Abilities", new AbilitySelectorViewModel(_dialogService, AbilityId.Levitate, _dataService.Ability)),
            new ListItem("Warrior Skills", new WarriorSkillSelectorViewModel(_dialogService, WarriorSkillId.Adrenaline, _dataService.WarriorSkill)),
            new ListItem("Move Ranges", new MoveRangeSelectorViewModel(_dialogService, MoveRangeId.Ahead1Tile, _dataService.MoveRange)),
            new ListItem("Evolution Table", new EvolutionTableViewModel(_dialogService, _dataService.Pokemon)),
            new ListItem("Warrior Name Table", new WarriorNameTableViewModel(_dialogService, _dataService.BaseWarrior)),
            new ListItem("Base Warrior", new BaseWarriorSelectorViewModel(_dialogService, WarriorId.PlayerMale_1, _dataService.BaseWarrior)),
            new ListItem("Max Link", new MaxLinkSelectorViewModel(_dialogService, WarriorId.PlayerMale_1, _dataService.MaxLink)),
            new ListItem("Scenario Warrior", new ScenarioWarriorSelectorViewModel(_dialogService, _dataService.ScenarioWarrior, scenario => new ScenarioWarriorViewModel(_dataService, scenario))),
            new ListItem("Scenario Pokemon", new ScenarioPokemonSelectorViewModel(_dialogService, _dataService.ScenarioPokemon, scenario => new ScenarioPokemonViewModel())),
            new ListItem("Scenario Appear Pokemon", new ScenarioAppearPokemonSelectorViewModel(_dialogService, ScenarioId.TheLegendOfRansei, _dataService.ScenarioAppearPokemon)),
            new ListItem("Scenario Kingdom", new ScenarioKingdomSelectorViewModel(_dialogService, 0, _dataService.ScenarioKingdom)),
            new ListItem("Event Speaker", new EventSpeakerSelectorViewModel(_dialogService, 0, _dataService.EventSpeaker))
        };
    }

    public void Save()
    {
        CurrentVm.Save();
    }

    private void CommitRom()
    {
        if (_dialogService.CommitToRom(Mod, out string romPath))
        {
            Save();
            try
            {
                _modService.Commit(Mod, romPath);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Writing To Rom",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
            }
        }
    }

    private void RunPlugin(PluginInfo chosen)
    {
        // first save
        Save();

        // then run plugin
        try
        {
            chosen.Plugin.Run(new PluginContext(_container, Mod));
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.Message,
                type: MessageBoxType.Error
                ));
        }

        // finally reload the items
        var currentItemId = CurrentListItem.ItemName;
        ReloadListItems();
        // make sure to not trigger Save by the CurrentVm setter
        _currentVm = ListItems.First(i => i.ItemName == currentItemId).ItemValue;
        RaisePropertyChanged(nameof(CurrentVm));
    }

    public PluginInfo SelectedPlugin
    {
        get => null;
        set
        {
            // prevent weird double trigger
            if (PluginPopupOpen)
            {
                PluginPopupOpen = false;
                RunPlugin(value);
            }
            
        }
    }

    private bool _pulginPopupOpen = false;
    public bool PluginPopupOpen
    {
        get => _pulginPopupOpen;
        set => RaiseAndSetIfChanged(ref _pulginPopupOpen, value);
    }
}
