using GongSolutions.Wpf.DragDrop;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorWorkspaceViewModel : ViewModelBase
{
    private bool _loading;
    private readonly SwMiniViewModel.Factory _itemFactory;
    private readonly SwKingdomMiniViewModel.Factory _kingdomItemFactory;
    private readonly SwSimpleKingdomMiniViewModel.Factory _simpleKingdomItemFactory;
    private static bool _showArmy = true;
    private static bool _showFree = false;
    private static bool _showUnassigned = false;
    private SwMiniViewModel _selectedItem;

    public ScenarioWarriorWorkspaceViewModel(
        SwMiniViewModel.Factory itemFactory,
        SwKingdomMiniViewModel.Factory kingdomItemFactory,
        SwSimpleKingdomMiniViewModel.Factory simpleKingdomItemFactory,
        IIdToNameService idToNameService)
    {
        _itemFactory = itemFactory;
        _kingdomItemFactory = kingdomItemFactory;
        _simpleKingdomItemFactory = simpleKingdomItemFactory;
        ItemDragHandler = new DragHandlerPro();
        ItemDropHandler = new DropHandlerPro();
        ItemClickedCommand = new RelayCommand<object>(ItemClicked);

        WarriorItems = idToNameService.GetComboBoxItemsExceptDefault<IBaseWarriorService>();
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();

        Items.CollectionChanged += Items_CollectionChanged;
        WildItems.CollectionChanged += Items_CollectionChanged;
        UnassignedItems.CollectionChanged += Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_loading || !(e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Move) || sender is not IList<object> items)
        {
            return;
        }

        var newItem = (SwMiniViewModel)e.NewItems[0];
        var oldClass = newItem.Class;
        var oldArmy = newItem.Army;

        // put into unassigned items list, change the army to default
        if (items == UnassignedItems)
        {
            newItem.Army = 17;
            newItem.Class = WarriorClassId.Default;
        }
        else
        {
            // find the new kingdom and army
            var newIndex = e.NewStartingIndex;
            var newKingdom = GetKingdom(newIndex, items);
            newItem.Kingdom = (int)newKingdom.Kingdom;
            newItem.Army = newKingdom.Army;

            if (items == WildItems)
            {
                if (newItem.Class != WarriorClassId.FreeWarrior && newItem.Class != WarriorClassId.Unknown_3 && newItem.Class != WarriorClassId.Unknown_4)
                {
                    newItem.Class = WarriorClassId.FreeWarrior;
                }
            }
            else if (items == Items)
            {
                if (newItem.Class != WarriorClassId.ArmyLeader && newItem.Class != WarriorClassId.ArmyMember)
                {
                    newItem.Class = WarriorClassId.ArmyMember;
                }
            }
        }

        // update the strengths of armies
        UpdateKingdomStrengths();

        // updating of leader
        // if the class is leader, and hasn't changed (changing class would have already done the updates)
        if (newItem.Class == WarriorClassId.ArmyLeader && oldClass == WarriorClassId.ArmyLeader)
        {
            UpdateLeaders();
        }
    }

    private static SwSimpleKingdomMiniViewModel GetKingdom(int index, IList<object> items)
    {
        // walk up the list from this index.
        for (int i = index - 1; i >= 0; i--)
        {
            if (items[i] is SwSimpleKingdomMiniViewModel vm)
            {
                return vm;
            }
        }
        throw new System.Exception("SwWorkspace: Something has gone wrong. Kingdom not found.");
    }

    public void UpdateKingdomStrengths()
    {
        foreach (var i in Items.OfType<SwKingdomMiniViewModel>())
        {
            i.UpdateStrength();
        }
    }

    public void UpdateLeaders()
    {
        foreach (var i in Items.OfType<SwKingdomMiniViewModel>())
        {
            i.UpdateLeader();
        }
    }

    private ScenarioPokemonViewModel _spVm;

    public void UpdateScenarioPokemonComboItemName(int id)
    {
        if (_childSpService.ValidateId(id))
        {
            var item = ScenarioPokemonItems.First(x => x.Id == id);
            item.UpdateName(_childSpService.IdToName(id));
        }
    }

    private IChildScenarioPokemonService _childSpService;

    public ScenarioWarriorWorkspaceViewModel Init(ScenarioPokemonViewModel spVm)
    {
        _spVm = spVm;
        _spVm.PropertyChanged += SpVm_PropertyChanged;
        return this;
    }

    public void SetModel(ScenarioId scenario, IChildScenarioWarriorService childSwService, IChildScenarioPokemonService childSpService)
    {
        _loading = true;

        _childSpService = childSpService;
        ScenarioPokemonItems = childSpService.GetComboBoxItemsExceptDefault();
        ScenarioPokemonItems.Insert(0, new SelectorComboBoxItem(1100, "No Pokemon"));

        foreach (var warrior in AllWarriors)
        {
            warrior.PropertyChanged -= WarriorItem_PropertyChanged;
        }

        Items.Clear();
        UnassignedItems.Clear();
        WildItems.Clear();
        foreach (var group in childSwService.Enumerate().GroupBy(x => x.Kingdom).OrderBy(x => x.Key))
        {
            Items.Add(_kingdomItemFactory().Init(scenario, group.Key));
            WildItems.Add(_simpleKingdomItemFactory().Init(group.Key));
            foreach (var scenarioWarrior in group.OrderBy(x => x.Class))
            {
                var item = _itemFactory().Init(scenarioWarrior, scenario, this, _spVm);
                item.PropertyChanged += WarriorItem_PropertyChanged;
                switch (scenarioWarrior.Class)
                {
                    case WarriorClassId.ArmyLeader:
                    case WarriorClassId.ArmyMember:
                        Items.Add(item);
                        break;
                    case WarriorClassId.FreeWarrior:
                    case WarriorClassId.Unknown_3:
                    case WarriorClassId.Unknown_4:
                        WildItems.Add(item);
                        break;
                    case WarriorClassId.Default:
                        UnassignedItems.Add(item);
                        break;
                    default:
                        throw new System.Exception("Unexpected warrior class id");
                }

            }
        }
        SelectedItem = Items.OfType<SwMiniViewModel>().FirstOrDefault();

        _loading = false;
    }

    private void WarriorItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SwMiniViewModel.Strength))
        {
            UpdateKingdomStrengths();
        }
    }

    private void SpVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ScenarioPokemonViewModel.Pokemon))
        {
            return;
        }

        var spid = SelectedItem.SelectedItem.ScenarioPokemonId;
        UpdateScenarioPokemonComboItemName(spid);
        foreach (var item in AllWarriors.Where(x => x.ScenarioPokemon == spid))
        {
            // if it's slot0 scenario pokemon matches, strength needs updating
            // avoid updating kingdom strengths multiple times by unsubscribing from this temporarily
            // while we do the strength update
            item.PropertyChanged -= WarriorItem_PropertyChanged;
            item.UpdateStrength();
            // we also update the first slot's image
            // this may need updating for multiple warriors
            // since the first pokemon of a warrior is always visible
            item.ScenarioPokemonSlots[0].UpdatePokemonImage();
            item.PropertyChanged += WarriorItem_PropertyChanged;
        }
        // the update pokemon image only applies to slot0, this may apply to other slots
        // but doesn't affect the images used in any other context.
        // the first image has already been updated above
        foreach (var slot in SelectedItem.ScenarioPokemonSlots.Skip(1).Where(x => x.ScenarioPokemonId == spid))
        {
            slot.UpdatePokemonImage();
        }

        UpdateKingdomStrengths();
    }

    private IEnumerable<SwMiniViewModel> AllWarriors => 
        Items.OfType<SwMiniViewModel>()
        .Concat(WildItems.OfType<SwMiniViewModel>())
        .Concat(UnassignedItems.OfType<SwMiniViewModel>());

    public ObservableCollection<object> Items { get; } = new();
    public ObservableCollection<object> WildItems { get; } = new();
    public ObservableCollection<object> UnassignedItems { get; } = new();
    public DragHandlerPro ItemDragHandler { get; }
    public DropHandlerPro ItemDropHandler { get; }
    public ICommand ItemClickedCommand { get; }
    public List<SelectorComboBoxItem> ScenarioPokemonItems { get; private set; }
    public List<SelectorComboBoxItem> WarriorItems { get; }
    public List<SelectorComboBoxItem> KingdomItems { get; }
    public List<SelectorComboBoxItem> ItemItems { get; }

    public bool ShowArmy
    {
        get => _showArmy;
        set => RaiseAndSetIfChanged(ref _showArmy, value);
    }
    public bool ShowFree
    {
        get => _showFree;
        set => RaiseAndSetIfChanged(ref _showFree, value);
    }
    public bool ShowUnassigned
    {
        get => _showUnassigned;
        set => RaiseAndSetIfChanged(ref _showUnassigned, value);
    }
    public SwMiniViewModel SelectedItem
    {
        get => _selectedItem;
        set 
        {
            if (RaiseAndSetIfChanged(ref _selectedItem, value))
            {
                value.SelectedItem?.UpdateNested();
            }
        }
    }

    private void ItemClicked(object sender)
    {
        if (sender is not SwMiniViewModel clickedVm)
        {
            return;
        }
        SelectedItem = clickedVm;
    }
}
