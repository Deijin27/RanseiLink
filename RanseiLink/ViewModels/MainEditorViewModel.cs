using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public record ListItem(string ItemName, ISaveableRefreshable ItemValue);

public delegate MainEditorViewModel MainEditorViewModelFactory(ModInfo mod);

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
                Save();
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
            new ListItem("Pokemon", _container.Resolve<PokemonSelectorViewModelFactory>()(_dataService.Pokemon)),
            new ListItem("Moves", _container.Resolve<MoveSelectorViewModelFactory>()(_dataService.Move)),
            new ListItem("Abilities", _container.Resolve<AbilitySelectorViewModelFactory>()(_dataService.Ability)),
            new ListItem("Warrior Skills", _container.Resolve<WarriorSkillSelectorViewModelFactory>()(_dataService.WarriorSkill)),
            new ListItem("Move Ranges", _container.Resolve<MoveRangeSelectorViewModelFactory>()(_dataService.MoveRange)),
            new ListItem("Evolution Table", _container.Resolve<EvolutionTableViewModelFactory>()(_dataService.Pokemon)),
            new ListItem("Warrior Name Table", _container.Resolve<WarriorNameTableViewModelFactory>()(_dataService.BaseWarrior)),
            new ListItem("Base Warrior", _container.Resolve<BaseWarriorSelectorViewModelFactory>()(_dataService.BaseWarrior)),
            new ListItem("Max Link", _container.Resolve<MaxLinkSelectorViewModelFactory>()(_dataService.MaxLink)),
            new ListItem("Scenario Warrior", _container.Resolve<ScenarioWarriorSelectorViewModelFactory>()(_dataService)),
            new ListItem("Scenario Pokemon", _container.Resolve<ScenarioPokemonSelectorViewModelFactory>()(_dataService.ScenarioPokemon)),
            new ListItem("Scenario Appear Pokemon", _container.Resolve<ScenarioAppearPokemonSelectorViewModelFactory>()(_dataService.ScenarioAppearPokemon)),
            new ListItem("Scenario Kingdom", _container.Resolve<ScenarioKingdomSelectorViewModelFactory>()(_dataService.ScenarioKingdom)),
            new ListItem("Event Speaker", _container.Resolve<EventSpeakerSelectorViewModelFactory>()(_dataService.EventSpeaker)),
            new ListItem("Items", _container.Resolve<ItemSelectorViewModelFactory>()(_dataService.Item)),
            new ListItem("Buildings", _container.Resolve<BuildingSelectorViewModelFactory>()(_dataService.Building)),
        };
    }

    public void Save()
    {
        if (!_blockSave)
        {
            CurrentVm?.Save();
        }
    }

    private void CommitRom()
    {
        if (!_dialogService.CommitToRom(Mod, out string romPath))
        {
            return;
        }

        Exception error = null;
        _dialogService.ProgressDialog(async (text, number) =>
        {
            text.Report("Saving...");
            Save();
            number.Report(20);
            text.Report("Patching rom...");
            try
            {
                _modService.Commit(Mod, romPath);
            }
            catch (Exception e)
            {
                error = e;
            }
            number.Report(100);
            text.Report("Patching Complete!");
            await Task.Delay(500);
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: "Error Writing To Rom",
                message: error.Message,
                type: MessageBoxType.Error
            ));
        }
    }

    private bool _blockSave = false;

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
        // block the save directly because reloading the list items triggers the CurrentVm setter
        _blockSave = true;
        // finally reload the items
        var currentItemId = CurrentListItem.ItemName;
        ReloadListItems();
        _currentVm = ListItems.First(i => i.ItemName == currentItemId).ItemValue;
        RaisePropertyChanged(nameof(CurrentVm));
        _blockSave = false;
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
