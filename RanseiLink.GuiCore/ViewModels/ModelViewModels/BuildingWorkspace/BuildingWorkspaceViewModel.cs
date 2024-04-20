using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace RanseiLink.GuiCore.ViewModels;

public class BuildingWorkspaceViewModel : ViewModelBase
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly ScenarioBuildingViewModel _scenarioBuildingVm;

    public ObservableCollection<object> Items { get; } = [];

    public BuildingWorkspaceViewModel(
        IBuildingService buildingService, 
        IKingdomService kingdomService,
        IJumpService jumpService, 
        IIdToNameService idToNameService, 
        ICachedSpriteProvider cachedSpriteProvider,
        IAnimGuiManager animManager,
        IScenarioBuildingService scenarioBuildingService,
        CopyPasteViewModel copyPasteVm)
    {

        BuildingItems = idToNameService.GetComboBoxItemsPlusDefault<IBuildingService>();
        JumpToBattleConfigCommand = new RelayCommand<BattleConfigId>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, (int)id));
        _cachedSpriteProvider = cachedSpriteProvider;
        CopyPasteVm = copyPasteVm;
        CopyPasteVm.ModelPasted += (_, __) => 
        {
            if (SelectedItem is BuildingViewModel bvm)
            {
                SelectItem(bvm);
            }
        };
        _scenarioBuildingVm = new ScenarioBuildingViewModel(scenarioBuildingService);
        ItemClickedCommand = new RelayCommand<object>(ItemClicked);
        // load the building view models
        var vms = new List<BuildingViewModel>();
        foreach (var id in buildingService.ValidIds())
        {
            var model = buildingService.Retrieve(id);
            var vm = new BuildingViewModel(_scenarioBuildingVm, this, kingdomService, cachedSpriteProvider, (BuildingId)id, model);
            vms.Add(vm);
        }

        // put the view models into the list. maybe this won't be a list long term
        foreach (var kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            Items.Add(new BuildingSimpleKingdomMiniViewModel(cachedSpriteProvider, kingdom));
            var intKingdom = (int)kingdom;
            int slot = 0;
            foreach (var vm in vms.Where(x => x.Kingdom == intKingdom))
            {
                vm.Slot = slot++;
                Items.Add(vm);
            }
        }
        SelectItem(vms.First());

        IconAnimVm = new(animManager, AnimationTypeId.IconInst, () => SelectedAnimation);
    }

    public ScenarioBuildingViewModel ScenarioBuildingVm => _scenarioBuildingVm;

    public List<SelectorComboBoxItem> BuildingItems { get; }

    public ICommand JumpToBattleConfigCommand { get; }

    public ICommand ItemClickedCommand { get; }

    private object _selectedItem;
    public object SelectedItem
    {
        get => _selectedItem;
    }

    private void ItemClicked(object? sender)
    {
        if (sender is BuildingViewModel buildingVm)
        {
            SelectItem(buildingVm);
        }
    }

    [MemberNotNull(nameof(_selectedItem))]
    private void SelectItem(BuildingViewModel buildingVm)
    {
        _selectedItem = buildingVm;
        CopyPasteVm.Model = buildingVm.Model;
        Notify(nameof(SelectedItem));
        _scenarioBuildingVm.SetSelected((KingdomId)buildingVm.Kingdom, buildingVm.Slot);
    }

    public void SelectById(int id)
    {
        foreach (var item in Items)
        {
            if (item is BuildingViewModel vm && vm.Id == id)
            {
                SelectItem(vm);
                break;
            }
        }
    }


    public AnimationViewModel? IconAnimVm { get; private set; }


    private int _selectedAnimation;
    public int SelectedAnimation
    {
        get => _selectedAnimation;
        set
        {
            if (Set(_selectedAnimation, value, v => _selectedAnimation = v))
            {
                Notify(nameof(SelectedAnimationImage));
                IconAnimVm?.OnIdChanged();
            }
        }
    }

    public object? SelectedAnimationImage => _cachedSpriteProvider.GetSprite(SpriteType.IconInstS, SelectedAnimation);

    public CopyPasteViewModel CopyPasteVm { get; }
}
