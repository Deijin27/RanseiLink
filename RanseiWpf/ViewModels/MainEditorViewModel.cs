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

        private readonly IWpfAppServices Services;
        public ModInfo Mod { get; }
        private readonly IDataService DataService;

        private IList<ListItem> _listItems;
        public IList<ListItem> ListItems
        {
            get => _listItems;
            set => RaiseAndSetIfChanged(ref _listItems, value);
        }

        public MainEditorViewModel(IWpfAppServices services, ModInfo mod)
        {
            Services = services;
            Mod = mod;
            DataService = services.CoreServices.DataService(Mod);

            ReloadListItems();

            CurrentVm = ListItems[0].ItemValue;
            CommitRomCommand = new RelayCommand(CommitRom);
            RandomizeCommand = new RelayCommand(Randomize);
        }

        private void ReloadListItems()
        {
            
            ListItems = new List<ListItem>()
            {
                new ListItem("Pokemon", new PokemonSelectorViewModel(PokemonId.Eevee, DataService.Pokemon)),
                new ListItem("Moves", new MoveSelectorViewModel(MoveId.Splash, DataService.Move)),
                new ListItem("Abilities", new AbilitySelectorViewModel(AbilityId.Levitate, DataService.Ability)),
                new ListItem("Warrior Skills", new WarriorSkillSelectorViewModel(WarriorSkillId.Adrenaline, DataService.WarriorSkill)),
                new ListItem("Move Ranges", new MoveRangeSelectorViewModel(MoveRangeId.Ahead1Tile, DataService.MoveRange)),
                new ListItem("Evolution Table", new EvolutionTableViewModel(DataService.Pokemon)),
                new ListItem("Scenario Warrior", new ScenarioWarriorSelectorViewModel(DataService.ScenarioWarrior, scenario => new ScenarioWarriorViewModel(DataService, scenario))),
                new ListItem("Scenario Pokemon", new ScenarioPokemonSelectorViewModel(DataService.ScenarioPokemon, scenario => new ScenarioPokemonViewModel())),
                new ListItem("Max Link", new WarriorMaxSyncSelectorViewModel(WarriorId.PlayerMale_1, DataService.MaxLink))
            };
        }

        public void Save()
        {
            CurrentVm.Save();
        }

        private void CommitRom()
        {
            if (Services.DialogService.CommitToRom(Mod, out string romPath))
            {
                Save();
                Services.CoreServices.ModService.Commit(Mod, romPath);
            }
        }

        private async void Randomize()
        {
            IRandomizer randomizer = new SimpleRandomizer();
            if (Services.DialogService.Randomize(randomizer))
            {
                // first save any unsaved changes
                Save();

                // then randomize
                var dialog = new LoadingDialog("Randomizing...");
                dialog.Owner = App.Current.MainWindow;
                dialog.Show();

                await Task.Run(() => randomizer.Apply(DataService));

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
