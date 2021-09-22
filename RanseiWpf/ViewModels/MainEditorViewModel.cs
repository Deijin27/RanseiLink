using Core.Enums;
using Core.Randomization;
using Core.Services;
using RanseiWpf.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class ListItem
    {
        public ListItem(string itemName, ISaveable itemValue)
        {
            ItemName = itemName;
            ItemValue = itemValue;
        }
        public string ItemName { get; }
        public ISaveable ItemValue { get; }
    }

    public class MainEditorViewModel : ViewModelBase, ISaveable
    {
        public ICommand CommitRomCommand { get; }
        public ICommand RandomizeCommand { get; }


        private ISaveable currentVm;
        public ISaveable CurrentVm
        {
            get => currentVm;
            set
            {
                if (currentVm != value)
                {
                    currentVm?.Save();
                    currentVm = value;
                    RaisePropertyChanged();
                }
            }
        }

        private readonly IWpfAppServices services;

        private IList<ListItem> _listItems;
        public IList<ListItem> ListItems
        {
            get => _listItems;
            set => RaiseAndSetIfChanged(ref _listItems, value);
        }

        public MainEditorViewModel(IWpfAppServices services, ModInfo mod)
        {
            this.services = services;
            IDataService dataService = services.CoreServices.DataService(mod);

            ListItems = new List<ListItem>()
            {
                new ListItem("Pokemon", new PokemonSelectorViewModel(PokemonId.Eevee, dataService.Pokemon)),
                new ListItem("Moves", new MoveSelectorViewModel(MoveId.Splash, dataService.Move)),
                new ListItem("Abilities", new AbilitySelectorViewModel(AbilityId.Levitate, dataService.Ability)),
                new ListItem("Warrior Skills", new WarriorSkillSelectorViewModel(WarriorSkillId.Adrenaline, dataService.WarriorSkill)),
                new ListItem("Move Ranges", new MoveRangeSelectorViewModel(MoveRangeId.Ahead1Tile, dataService.MoveRange)),
                new ListItem("Evolution Table", new EvolutionTableViewModel(dataService.EvolutionTable)),
                new ListItem("Scenario Pokemon", new ScenarioPokemonSelectorViewModel(dataService.ScenarioPokemon)),
                new ListItem("Max Link", new WarriorMaxSyncSelectorViewModel(WarriorId.PlayerMale_1, dataService.MaxLink))
            };

            CurrentVm = ListItems[0].ItemValue;
        }

        public void Save()
        {
            CurrentVm.Save();
        }
    }
}
