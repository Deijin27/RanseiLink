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
        UpdateStrengths();

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

    public void UpdateStrengths()
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

    public void SetModel(ScenarioId scenario, IChildScenarioWarriorService childSwService, IChildScenarioPokemonService childSpService, ScenarioPokemonViewModel spVm)
    {
        _loading = true;
        ScenarioPokemonItems = childSpService.GetComboBoxItemsExceptDefault();
        ScenarioPokemonItems.Insert(0, new SelectorComboBoxItem(1100, "No Pokemon"));

        Items.Clear();
        UnassignedItems.Clear();
        WildItems.Clear();
        foreach (var group in childSwService.Enumerate().GroupBy(x => x.Kingdom).OrderBy(x => x.Key))
        {
            Items.Add(_kingdomItemFactory().Init(scenario, group.Key));
            WildItems.Add(_simpleKingdomItemFactory().Init(group.Key));
            foreach (var scenarioWarrior in group.OrderBy(x => x.Class))
            {
                var item = _itemFactory().Init(scenarioWarrior, scenario, this, spVm); // <-- TODO: share a single spvm?
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
