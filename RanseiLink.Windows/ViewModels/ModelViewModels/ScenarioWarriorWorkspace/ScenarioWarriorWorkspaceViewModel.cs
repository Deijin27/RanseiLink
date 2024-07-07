#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RanseiLink.Windows.ViewModels;

public class ScenarioWarriorWorkspaceViewModel : ViewModelBase
{
    private bool _loading;
    private readonly SwMiniViewModel.Factory _itemFactory;
    private readonly SwKingdomMiniViewModel.Factory _kingdomItemFactory;
    private readonly SwSimpleKingdomMiniViewModel.Factory _simpleKingdomItemFactory;
    private static bool _showArmy = true;
    private static bool _showFree = false;
    private static bool _showUnassigned = false;
    private object? _selectedItem;

    public ScenarioWarriorWorkspaceViewModel(
        SwMiniViewModel.Factory itemFactory,
        SwKingdomMiniViewModel.Factory kingdomItemFactory,
        SwSimpleKingdomMiniViewModel.Factory simpleKingdomItemFactory,
        IIdToNameService idToNameService,
        IJumpService jumpService)
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
        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(WarriorWorkspaceModule.Id, id));
        JumpToMaxLinkCommand = new RelayCommand<int>(id => jumpService.JumpTo(MaxLinkSelectorEditorModule.Id, id));
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemSelectorEditorModule.Id, id));

        Items.CollectionChanged += Items_CollectionChanged;
        WildItems.CollectionChanged += Items_CollectionChanged;
        UnassignedItems.CollectionChanged += Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_loading)
        {
            return;
        }

        if (!(e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Move))
        {
            return;
        }

        if (sender is not IList<object> items)
        {
            return;
        }

        if (e.NewItems == null)
        {
            return;
        }

        var newItem = e.NewItems[0] as SwMiniViewModel;
        if (newItem == null)
        {
            return;
        }
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
                if (newItem.Class != WarriorClassId.FreeWarrior_1 && newItem.Class != WarriorClassId.FreeWarrior_2 && newItem.Class != WarriorClassId.FreeWarrior_3)
                {
                    newItem.Class = WarriorClassId.FreeWarrior_1;
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

    private ScenarioPokemonViewModel _spVm = null!;

    public void UpdateScenarioPokemonComboItemName(int id)
    {
        if (_childSpService.ValidateId(id))
        {
            var item = ScenarioPokemonItems.First(x => x.Id == id);
            item.UpdateName(_childSpService.IdToName(id));
        }
    }

    private IChildScenarioPokemonService _childSpService = null!;

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
        foreach (var kingdomItem in Items.OfType<SwKingdomMiniViewModel>())
        {
            kingdomItem.PropertyChanged -= KingdomItem_PropertyChanged;
        }

        Items.Clear();
        UnassignedItems.Clear();
        WildItems.Clear();

        foreach (var kingdom in EnumUtil.GetValues<KingdomId>())
        {
            var group = childSwService.Enumerate().Select((warrior, id) => (warrior, id)).Where(x => x.warrior.Kingdom == kingdom);
            var kingdomItem = _kingdomItemFactory().Init(scenario, kingdom, ItemClickedCommand);
            kingdomItem.PropertyChanged += KingdomItem_PropertyChanged;
            Items.Add(kingdomItem);
            WildItems.Add(_simpleKingdomItemFactory().Init(kingdom));
            foreach (var scenarioWarrior in group.OrderBy(x => x.warrior.Class))
            {
                var item = _itemFactory().Init(scenarioWarrior.id, scenarioWarrior.warrior, scenario, this, _spVm);
                item.PropertyChanged += WarriorItem_PropertyChanged;
                switch (scenarioWarrior.warrior.Class)
                {
                    case WarriorClassId.ArmyLeader:
                    case WarriorClassId.ArmyMember:
                        Items.Add(item);
                        break;
                    case WarriorClassId.FreeWarrior_1:
                    case WarriorClassId.FreeWarrior_2:
                    case WarriorClassId.FreeWarrior_3:
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

    private void KingdomItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // update the army of the warriors in a kingdom
        // to keep in sync with the army of the kingdom
        if (e.PropertyName != nameof(SwKingdomMiniViewModel.Army))
        {
            return;
        }
        if (sender is not SwKingdomMiniViewModel kingdomItem)
        {
            return;
        }
        // only need to recurse ones that will have an army
        foreach (var warrior in Items.OfType<SwMiniViewModel>().Where(x => x.Kingdom == (int)kingdomItem.Kingdom))
        {
            warrior.Army = kingdomItem.Army;
        }

        // since the army has changed, the leader of this kingdom could have changed
        // and if this kingdom contains a leader, the leader of other kingdoms could have changed too.
        UpdateLeaders();
    }

    private void WarriorItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SwMiniViewModel.Strength))
        {
            UpdateKingdomStrengths();
        }
    }

    private void SpVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ScenarioPokemonViewModel.Pokemon))
        {
            return;
        }

        if (SelectedItem is not SwMiniViewModel selectedSw)
        {
            // this should never be the case
            return;
        }

        if (selectedSw.SelectedItem == null)
        {
            return;
        }

        var spid = selectedSw.SelectedItem.ScenarioPokemonId;
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
        foreach (var slot in selectedSw.ScenarioPokemonSlots.Skip(1).Where(x => x.ScenarioPokemonId == spid))
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
    public List<SelectorComboBoxItem> ScenarioPokemonItems { get; private set; } = null!;
    public List<SelectorComboBoxItem> WarriorItems { get; }
    public List<SelectorComboBoxItem> KingdomItems { get; }
    public List<SelectorComboBoxItem> ItemItems { get; }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }
    public ICommand JumpToItemCommand { get; }

    public bool ShowArmy
    {
        get => _showArmy;
        set => SetProperty(ref _showArmy, value);
    }
    public bool ShowFree
    {
        get => _showFree;
        set => SetProperty(ref _showFree, value);
    }
    public bool ShowUnassigned
    {
        get => _showUnassigned;
        set => SetProperty(ref _showUnassigned, value);
    }
    public object? SelectedItem
    {
        get => _selectedItem;
        set 
        {
            if (SetProperty(ref _selectedItem, value))
            {
                if (value is SwMiniViewModel spVm)
                {
                    spVm.SelectedItem?.UpdateNested();
                }
            }
        }
    }

    private void ItemClicked(object? sender)
    {
        if (sender is SwMiniViewModel)
        {
            SelectedItem = sender;
        }
        else if (sender is SwKingdomMiniViewModel swkvm && swkvm.Kingdom != KingdomId.Default)
        {
            // default kingdom doesn't have a ScenarioKingdom slot value to be modified
            SelectedItem = sender;
        }
    }
}
