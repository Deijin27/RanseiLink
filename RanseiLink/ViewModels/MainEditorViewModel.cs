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

public record ListItem(string ItemName, MainEditorPage ItemValue);

public delegate MainEditorViewModel MainEditorViewModelFactory(ModInfo mod);

public enum MainEditorPage
{
    Pokemon,
    Move,
    Ability,
    WarriorSkill,
    MoveRange,
    EvolutionTable,
    WarriorNameTable,
    BaseWarrior,
    MaxLink,
    ScenarioWarrior,
    ScenarioPokemon,
    ScenarioAppearPokemon,
    ScenarioKingdom,
    EventSpeaker,
    Item,
    Building,
}

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
            Save();
            _currentVm = value;
            _currentVm?.Refresh();
            RaisePropertyChanged();
        }
    }

    private MainEditorPage _currentPage = MainEditorPage.Pokemon;
    public MainEditorPage CurrentPage
    {
        get => _currentPage;
        set
        {
            if (RaiseAndSetIfChanged(ref _currentPage, value))
            {
                CurrentVm = SelectViewModel(value);
            }
        }
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }
    public ModInfo Mod { get; }
    public IList<ListItem> ListItems { get; }

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

        ListItems = new List<ListItem>()
        {
            new ListItem("Pokemon", MainEditorPage.Pokemon),
            new ListItem("Moves", MainEditorPage.Move),
            new ListItem("Abilities", MainEditorPage.Ability),
            new ListItem("Warrior Skills", MainEditorPage.WarriorSkill),
            new ListItem("Move Ranges", MainEditorPage.MoveRange),
            new ListItem("Evolution Table", MainEditorPage.EvolutionTable),
            new ListItem("Warrior Name Table", MainEditorPage.WarriorNameTable),
            new ListItem("Base Warrior", MainEditorPage.BaseWarrior),
            new ListItem("Max Link", MainEditorPage.MaxLink),
            new ListItem("Scenario Warrior", MainEditorPage.ScenarioWarrior),
            new ListItem("Scenario Pokemon", MainEditorPage.ScenarioPokemon),
            new ListItem("Scenario Appear Pokemon", MainEditorPage.ScenarioAppearPokemon),
            new ListItem("Scenario Kingdom", MainEditorPage.ScenarioKingdom),
            new ListItem("Event Speaker", MainEditorPage.EventSpeaker),
            new ListItem("Items", MainEditorPage.Item),
            new ListItem("Buildings", MainEditorPage.Building),
        };

        ReloadViewModels();
        CurrentVm = SelectViewModel(_currentPage);

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

    private void ReloadViewModels()
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
    }

    private ISaveableRefreshable SelectViewModel(MainEditorPage id)
    {
        return id switch
        {
            MainEditorPage.Pokemon => PokemonSelector,
            MainEditorPage.Move => MoveSelector,
            MainEditorPage.Ability => AbilitySelector,
            MainEditorPage.WarriorSkill => WarriorSkillSelector,
            MainEditorPage.MoveRange => MoveRangeSelector,
            MainEditorPage.EvolutionTable => EvolutionTableViewModel,
            MainEditorPage.WarriorNameTable => WarriorNameTableViewModel,
            MainEditorPage.BaseWarrior => BaseWarriorSelector,
            MainEditorPage.MaxLink => MaxLinkSelector,
            MainEditorPage.ScenarioWarrior => ScenarioWarriorSelector,
            MainEditorPage.ScenarioPokemon => ScenarioPokemonSelector,
            MainEditorPage.ScenarioAppearPokemon => ScenarioAppearPokemonSelector,
            MainEditorPage.ScenarioKingdom => ScenarioKingdomSelector,
            MainEditorPage.EventSpeaker => EventSpeakerSelector,
            MainEditorPage.Item => ItemSelector,
            MainEditorPage.Building => BuildingSelector,
            _ => throw new ArgumentException($"Invalid {nameof(MainEditorPage)} enum value"),
        };
    }

    public void Save()
    {
        CurrentVm?.Save();
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
                await Task.Run(() => _modService.Commit(Mod, romPath));
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
        ReloadViewModels();
        _currentVm = null;
        CurrentVm = SelectViewModel(_currentPage);
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
