#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

[EditorModule]
public class PokemonWorkspaceModule : BaseWorkspaceEditorModule<IPokemonService>
{
    public const string Id = "pokemon_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Pokemon";

    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<PokemonViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id => 
                new PokemonMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class WarriorWorkspaceModule : BaseWorkspaceEditorModule<IBaseWarriorService>
{
    public const string Id = "warrior_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Warrior";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<BaseWarriorViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id => 
                new WarriorMiniViewModel(sp, _service, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class MoveWorkspaceModule : BaseWorkspaceEditorModule<IMoveService>
{
    public const string Id = "move_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Move";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<MoveViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new MoveMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class MoveAnimationSelectorEditorModule : EditorModule
{
    public const string Id = "move_animation_list";
    public override string UniqueId => Id;
    public override string ListName => "Move Animation";
    private IMoveAnimationService _service = null!;
    private MoveAnimationCollectionViewModel? _viewModel;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IMoveAnimationService>();
        _viewModel = new MoveAnimationCollectionViewModel();
        _viewModel.Init(_service);
    }

    public override object? ViewModel => _viewModel;

    public override void Deactivate() => _service?.Save();
    public override void OnPatchingRom() => _service?.Save();
}

[EditorModule]
public class AbilityWorkspaceEditorModule : BaseWorkspaceEditorModule<IAbilityService>
{
    public const string Id = "ability_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Ability";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        var ps = modServices.Get<IPokemonService>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<AbilityViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new AbilityMiniViewModel(sp, ps, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class WarriorSkillWorkspaceEditorModule : BaseWorkspaceEditorModule<IWarriorSkillService>
{
    public const string Id = "warrior_skill_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Warrior Skill";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        var ps = modServices.Get<IBaseWarriorService>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<WarriorSkillViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new WarriorSkillMiniViewModel(sp, ps, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

public class MoveRangeWorkspaceModule : BaseWorkspaceEditorModule<IMoveRangeService>
{
    public const string Id = "move_range_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Move Range";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        var vm = modServices.Get<MoveRangeViewModel>();
        var ns = modServices.Get<INicknameService>();
        vm.Initialise(nameof(MoveRangeId));
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            vm,
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new MoveRangeMiniViewModel(nameof(MoveRangeId), ns, sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

public class GimmickRangeWorkspaceModule : BaseWorkspaceEditorModule<IGimmickRangeService>
{
    public const string Id = "gimmick_range_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick Range";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        var vm = modServices.Get<MoveRangeViewModel>();
        var ns = modServices.Get<INicknameService>();
        vm.Initialise(nameof(GimmickRangeId));
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            vm,
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new MoveRangeMiniViewModel(nameof(GimmickRangeId), ns, sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class WarriorNameTableEditorModule : EditorModule
{
    public const string Id = "warrior_name_table";
    public override string UniqueId => Id;
    public override string ListName => "Warrior Name Table";
    public override object? ViewModel => _viewModel;

    private WarriorNameTableViewModel? _viewModel;
    private IBaseWarriorService? _service;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IBaseWarriorService>();
        _viewModel = new WarriorNameTableViewModel();
        _viewModel.SetModel(_service.NameTable);
    }
    public override void Deactivate() => _service?.Save();
    public override void OnPatchingRom() => _service?.Save();
}

[EditorModule]
public class MaxLinkSelectorEditorModule : BaseSelectorEditorModule<IMaxLinkService>
{
    public const string Id = "max_link_selector";
    public override string UniqueId => Id;
    public override string ListName => "Max Link (Warrior)";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<MaxLinkWarriorViewModel>();
        var items = modServices.Get<IBaseWarriorService>().GetComboBoxItemsExceptDefault();
        SelectorViewModel = _selectorVmFactory.Create(_service, items, vm, id => vm.SetModel(id, _service.Retrieve(id)), _service.ValidateId, scrollEnabled: false);
    }
}

[EditorModule]
public class MaxLinkPokemonSelectorEditorModule : BaseSelectorEditorModule<IMaxLinkService>
{
    public const string Id = "max_link_pokemon_selector";
    public override string UniqueId => Id;
    public override string ListName => "Max Link (Pokemon)";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<MaxLinkPokemonViewModel>();
        var pokemonService = modServices.Get<IPokemonService>();
        var items = pokemonService.GetComboBoxItemsExceptDefault();
        SelectorViewModel = _selectorVmFactory.Create(null, items, vm, id => vm.SetModel(id, _service), pokemonService.ValidateId, scrollEnabled: false);
    }
}

[EditorModule]
public class ScenarioAppearPokemonSelectorEditorModule : BaseSelectorEditorModule<IScenarioAppearPokemonService>
{
    public const string Id = "scenario_appear_pokemon_selector";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Appear Pokemon";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<ScenarioAppearPokemonViewModel>();
        SelectorViewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel(_service.Retrieve(id)), scrollEnabled: false);
    }
}

[EditorModule]
public class EventSpeakerWorkspaceModule : BaseWorkspaceEditorModule<IEventSpeakerService>
{
    public const string Id = "event_speaker_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Event Speaker";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<EventSpeakerViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new EventSpeakerMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class ItemWorkspaceModule : BaseWorkspaceEditorModule<IItemService>
{
    public const string Id = "item_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Item";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<ItemViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new ItemMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class BuildingWorkspaceEditorModule : BaseWorkspaceEditorModule<IBuildingService>
{
    public const string Id = "building_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Building";

    private IScenarioBuildingService? _scenarioBuildingService;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _scenarioBuildingService = modServices.Get<IScenarioBuildingService>();
        var sp = modServices.Get<ICachedSpriteProvider>();
        var idns = modServices.Get<IIdToNameService>();

        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<BuildingViewModel>(),
            _service,
            command =>
            {
                var lst = new List<IMiniViewModel>();
                foreach (var kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
                {
                    lst.Add(new BuildingSimpleKingdomMiniViewModel(sp, idns, kingdom));
                    foreach (var id in _service.ValidIds())
                    {
                        var model = _service.Retrieve(id);
                        if (model.Kingdom == kingdom)
                        {
                            lst.Add(new BuildingMiniViewModel(sp, model, id, command));
                        }
                    }
                }
                return lst;
            }
            );
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _scenarioBuildingService?.Save();
    }

    public override void OnPatchingRom()
    {
        base.OnPatchingRom();
        _scenarioBuildingService?.Save();
    }
}

[EditorModule]
public class MsgGridEditorModule : EditorModule
{
    public const string Id = "msg_grid";
    public override string UniqueId => Id;
    public override string ListName => "Text";
    public override object? ViewModel => _viewModel;
    private MsgGridViewModel? _viewModel;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _viewModel = modServices.Get<MsgGridViewModel>();
    }
    public override void Deactivate() => _viewModel?.UnhookEvents();
}

[EditorModule]
public class GimmickWorkspaceEditorModule : BaseWorkspaceEditorModule<IGimmickService>
{
    public const string Id = "gimmick_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<GimmickViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new GimmickMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class GimmickObjectSelectorEditorModule : BaseSelectorEditorModule<IGimmickObjectService>
{
    public const string Id = "gimmick_object_selector";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick 3D Model";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<GimmickObjectViewModel>();
        var nn = modServices.Get<INicknameService>();
        var comboItems = _service.ValidIds()
            .Select(x => (SelectorComboBoxItem)new NicknamedSelectorComboBoxItem(x, nn, nameof(GimmickObjectId)))
            .ToList();
        SelectorViewModel = _selectorVmFactory.Create(_service, comboItems, vm, id => vm.SetModel((GimmickObjectId)id, _service.Retrieve(id)), _service.ValidateId);
    }
}

[EditorModule]
public class EpisodeWorkspaceEditorModule : BaseWorkspaceEditorModule<IEpisodeService>
{
    public const string Id = "episode_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Episode";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        var bw = modServices.Get<IBaseWarriorService>();
        var sw = modServices.Get<IScenarioWarriorService>();
        var sk = modServices.Get<IScenarioKingdomService>();
        var ms = modServices.Get<ICachedMsgBlockService>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<EpisodeViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new EpisodeMiniViewModel(sp, bw, sw, sk, ms, _service.Retrieve(id), id, command)).ToList()
            );
        WorkspaceViewModel.LeftColumnWidth = 236;
    }
}

[EditorModule]
public class KingdomWorkspaceEditorModule : BaseWorkspaceEditorModule<IKingdomService>
{
    public const string Id = "kingdom_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Kingdom";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<KingdomViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new KingdomMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class BattleConfigWorkspaceEditorModule : BaseWorkspaceEditorModule<IBattleConfigService>
{
    public const string Id = "battle_config_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Battle Config";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<BattleConfigViewModel>();
        var nn = modServices.Get<INicknameService>();
        var cs = modServices.Get<ICachedSpriteProvider>();

        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<BattleConfigViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new BattleConfigMiniViewModel(cs, nn, _service.Retrieve(id), id, command)).ToList()
            );
        WorkspaceViewModel.LeftColumnWidth = 206;

    }
}

[EditorModule]
public class SpriteEditorModule : EditorModule
{
    public const string Id = "sprites";
    public override string UniqueId => Id;
    public override string ListName => "Sprites";
    public override object? ViewModel => _viewModel;

    private SpriteTypeViewModel? _viewModel;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _viewModel = modServices.Get<SpriteTypeViewModel>();
    }
}

[EditorModule]
public class BannerEditorModule : EditorModule
{
    public const string Id = "banner";
    public override string UniqueId => Id;
    public override string ListName => "Banner";
    public override object? ViewModel => _viewModel;

    private BannerViewModel? _viewModel;
    private IBannerService? _service;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IBannerService>();
        _viewModel = modServices.Get<BannerViewModel>();
    }
    public override void Deactivate() => _service?.Save();
    public override void OnPatchingRom() => _service?.Save();
}


[EditorModule]
public class MapSelectorEditorModule : BaseWorkspaceEditorModule<IMapService>
{
    public const string Id = "map_workspace";

    public override string UniqueId => Id;
    public override string ListName => "Map";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var sp = modServices.Get<ICachedSpriteProvider>();
        var cs = modServices.Get<ICachedSpriteProvider>();
        var nn = modServices.Get<INicknameService>();
        var nestedVm = modServices.Get<MapViewModel>();
        WorkspaceViewModel = _selectorVmFactory.CreateWorkspace(
            nestedVm,
            _service,
            command => _service.GetMapIds().Select<MapId, IMiniViewModel>(id =>
                new MapMiniViewModel(cs, nn, _service.Retrieve((int)id), id, command)).ToList()
            );
        WorkspaceViewModel.ScrollBig = false;
        WorkspaceViewModel.LeftColumnWidth = 176;
        nestedVm.RequestSave += NestedVm_RequestSave;
        nestedVm.RequestReload += NestedVm_RequestReload;
    }

    private void NestedVm_RequestReload(MapId id, PSLM model)
    {
        if (_service == null || WorkspaceViewModel == null || WorkspaceViewModel.SelectedMiniVm == null)
        {
            return;
        }
        var intId = (int)id;
        _service.Reload(id);
        var newModel = _service.Retrieve((int)id);
        ((MapMiniViewModel)WorkspaceViewModel.SelectedMiniVm).SetModel(newModel);
        WorkspaceViewModel.BigViewModel.SetModel(intId, newModel);
    }

    private void NestedVm_RequestSave(MapId id, PSLM model)
    {
        _service?.Save(id);
    }
}
