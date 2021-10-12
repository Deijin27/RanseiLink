using Core.Enums;
using Core.Randomization;
using Core.Services;
using RanseiWpf.Dialogs;
using RanseiWpf.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class ListItem
    {
        public ListItem(string itemName, ISaveableRefreshable itemValue)
        {
            ItemName = itemName;
            ItemValue = itemValue;
        }
        public string ItemName { get; }
        public ISaveableRefreshable ItemValue { get; }
    }

    public class MainEditorViewModel : ViewModelBase, ISaveable
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IModService _modService;

        public ICommand CommitRomCommand { get; }
        public ICommand RandomizeCommand { get; }

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
            var dataServiceFactory = container.Resolve<IDataServiceFactory>();
            _dialogService = container.Resolve<IDialogService>();
            _modService = container.Resolve<IModService>();

            Mod = mod;
            _dataService = dataServiceFactory.Create(Mod);

            ReloadListItems();

            CurrentVm = ListItems[0].ItemValue;
            CommitRomCommand = new RelayCommand(CommitRom);
            RandomizeCommand = new RelayCommand(Randomize);
        }

        private void ReloadListItems()
        {
            ListItems = new List<ListItem>()
            {
                new ListItem("Pokemon", new PokemonSelectorViewModel(PokemonId.Eevee, _dataService.Pokemon)),
                new ListItem("Moves", new MoveSelectorViewModel(MoveId.Splash, _dataService.Move)),
                new ListItem("Abilities", new AbilitySelectorViewModel(AbilityId.Levitate, _dataService.Ability)),
                new ListItem("Warrior Skills", new WarriorSkillSelectorViewModel(WarriorSkillId.Adrenaline, _dataService.WarriorSkill)),
                new ListItem("Move Ranges", new MoveRangeSelectorViewModel(MoveRangeId.Ahead1Tile, _dataService.MoveRange)),
                new ListItem("Evolution Table", new EvolutionTableViewModel(_dataService.Pokemon)),
                new ListItem("Warrior Name Table", new WarriorNameTableViewModel(_dataService.BaseWarrior)),
                new ListItem("Base Warrior", new BaseWarriorSelectorViewModel(WarriorId.PlayerMale_1, _dataService.BaseWarrior)),
                new ListItem("Scenario Warrior", new ScenarioWarriorSelectorViewModel(_dataService.ScenarioWarrior, scenario => new ScenarioWarriorViewModel(_dataService, scenario))),
                new ListItem("Scenario Pokemon", new ScenarioPokemonSelectorViewModel(_dataService.ScenarioPokemon, scenario => new ScenarioPokemonViewModel())),
                new ListItem("Scenario Appear Pokemon", new ScenarioAppearPokemonSelectorViewModel(ScenarioId.TheLegendOfRansei, _dataService.ScenarioAppearPokemon)),
                new ListItem("Max Link", new WarriorMaxSyncSelectorViewModel(WarriorId.PlayerMale_1, _dataService.MaxLink))
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
                _modService.Commit(Mod, romPath);
            }
        }

        private async void Randomize()
        {
            IRandomizer randomizer = new SimpleRandomizer();
            if (_dialogService.Randomize(randomizer))
            {
                // first save any unsaved changes
                Save();

                // then randomize
                var dialog = new LoadingDialog("Randomizing...");
                dialog.Owner = App.Current.MainWindow;
                dialog.Show();

                await Task.Run(() => randomizer.Apply(_dataService));

                // finally reload the items
                var currentItemId = CurrentListItem.ItemName;
                ReloadListItems();
                // make sure to not trigger Save by the CurrentVm setter
                _currentVm = ListItems.First(i => i.ItemName == currentItemId).ItemValue;
                RaisePropertyChanged(nameof(CurrentVm));

                dialog.Close();
            }
        }
    }
}
