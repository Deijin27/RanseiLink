using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
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
    private readonly IEditorContext _editorContext;

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
        var editorContextFactory = container.Resolve<EditorContextFactory>();
        _dialogService = container.Resolve<IDialogService>();
        _modService = container.Resolve<IModService>();

        PluginItems = container.Resolve<IPluginLoader>().LoadPlugins(out var _);

        Mod = mod;
        _dataService = dataServiceFactory(Mod);
        _editorContext = editorContextFactory(_dataService, this);

        ReloadListItems();

        CurrentVm = ListItems[0].ItemValue;
        CommitRomCommand = new RelayCommand(CommitRom);
    }

    public PokemonSelectorViewModel PokemonSelector { get; private set; }
    public MoveSelectorViewModel MoveSelector { get; private set; }
    public AbilitySelectorViewModel AbilitySelector { get; private set; }
    public WarriorSkillSelectorViewModel WarriorSkillSelector { get; private set; }
    public MoveRangeSelectorViewModel MoveRangeSelector { get; private set; }
    public EvolutionTableViewModel EvolutionTableViewModel { get; private set; }
    public WarriorNameTableViewModel WarriorNameTableViewModel { get; private set; }
    public BaseWarriorSelectorViewModel BaseWarriorSelector { get; private set; }
    public MaxLinkSelectorViewModel MaxLinkSelector { get; private set; }
    public ScenarioWarriorSelectorViewModel ScenarioWarriorSelector { get; private set; }
    public ScenarioPokemonSelectorViewModel ScenarioPokemonSelector { get; private set; }
    public ScenarioAppearPokemonSelectorViewModel ScenarioAppearPokemonSelector { get; private set; }
    public ScenarioKingdomSelectorViewModel ScenarioKingdomSelector { get; private set; }
    public EventSpeakerSelectorViewModel EventSpeakerSelector { get; private set; }
    public ItemSelectorViewModel ItemSelector { get; private set; }
    public BuildingSelectorViewModel BuildingSelector { get; private set; }

    private void ReloadListItems()
    {
        PokemonSelector = _container.Resolve<PokemonSelectorViewModelFactory>()(_editorContext);
        MoveSelector = _container.Resolve<MoveSelectorViewModelFactory>()(_editorContext);
        AbilitySelector = _container.Resolve<AbilitySelectorViewModelFactory>()(_editorContext);
        WarriorSkillSelector = _container.Resolve<WarriorSkillSelectorViewModelFactory>()(_editorContext);
        MoveRangeSelector = _container.Resolve<MoveRangeSelectorViewModelFactory>()(_editorContext);
        EvolutionTableViewModel = _container.Resolve<EvolutionTableViewModelFactory>()(_editorContext);
        WarriorNameTableViewModel = _container.Resolve<WarriorNameTableViewModelFactory>()(_editorContext);
        BaseWarriorSelector = _container.Resolve<BaseWarriorSelectorViewModelFactory>()(_editorContext);
        MaxLinkSelector = _container.Resolve<MaxLinkSelectorViewModelFactory>()(_editorContext);
        ScenarioWarriorSelector = _container.Resolve<ScenarioWarriorSelectorViewModelFactory>()(_editorContext);
        ScenarioPokemonSelector = _container.Resolve<ScenarioPokemonSelectorViewModelFactory>()(_editorContext);
        ScenarioAppearPokemonSelector = _container.Resolve<ScenarioAppearPokemonSelectorViewModelFactory>()(_editorContext);
        ScenarioKingdomSelector = _container.Resolve<ScenarioKingdomSelectorViewModelFactory>()(_editorContext);
        EventSpeakerSelector = _container.Resolve<EventSpeakerSelectorViewModelFactory>()(_editorContext);
        ItemSelector = _container.Resolve<ItemSelectorViewModelFactory>()(_editorContext);
        BuildingSelector = _container.Resolve<BuildingSelectorViewModelFactory>()(_editorContext);

        ListItems = new List<ListItem>()
        {
            new ListItem("Pokemon", PokemonSelector),
            new ListItem("Moves", MoveSelector),
            new ListItem("Abilities", AbilitySelector),
            new ListItem("Warrior Skills", WarriorSkillSelector),
            new ListItem("Move Ranges", MoveRangeSelector),
            new ListItem("Evolution Table", EvolutionTableViewModel),
            new ListItem("Warrior Name Table", WarriorNameTableViewModel),
            new ListItem("Base Warrior", BaseWarriorSelector),
            new ListItem("Max Link", MaxLinkSelector),
            new ListItem("Scenario Warrior", ScenarioWarriorSelector),
            new ListItem("Scenario Pokemon", ScenarioPokemonSelector),
            new ListItem("Scenario Appear Pokemon", ScenarioAppearPokemonSelector),
            new ListItem("Scenario Kingdom", ScenarioKingdomSelector),
            new ListItem("Event Speaker", EventSpeakerSelector),
            new ListItem("Items", ItemSelector),
            new ListItem("Buildings", BuildingSelector),
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
        var currentItemType = CurrentVm.GetType();
        ReloadListItems();
        _currentVm = ListItems.First(i => i.GetType() == currentItemType).ItemValue;
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
