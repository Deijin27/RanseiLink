#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;

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
        _viewModel = _selectorVmFactory.CreateWorkspace(
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
        _viewModel = _selectorVmFactory.CreateWorkspace(
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
        _viewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<MoveViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new MoveMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class AbilitySelectorEditorModule : BaseSelectorEditorModule<IAbilityService>
{
    public const string Id = "ability_selector";
    public override string UniqueId => Id;
    public override string ListName => "Ability";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<AbilityViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel((AbilityId)id, _service.Retrieve(id)));
    }
}

[EditorModule]
public class WarriorSkillSelectorEditorModule : BaseSelectorEditorModule<IWarriorSkillService>
{
    public const string Id = "warrior_skill_selector";
    public override string UniqueId => Id;
    public override string ListName => "Warrior Skill";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<WarriorSkillViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel((WarriorSkillId)id, _service.Retrieve(id)));
    }
}

[EditorModule]
public class MoveRangeSelectorEditorModule : BaseSelectorEditorModule<IMoveRangeService>
{
    public const string Id = "move_range_selector";
    public override string UniqueId => Id;
    public override string ListName => "Move Range";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<MoveRangeViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
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
        _viewModel = _selectorVmFactory.Create(_service, items, vm, id => vm.SetModel(id, _service.Retrieve(id)), _service.ValidateId, scrollEnabled: false);
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
        _viewModel = _selectorVmFactory.Create(null, items, vm, id => vm.SetModel(id, _service), pokemonService.ValidateId, scrollEnabled: false);
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
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel(_service.Retrieve(id)), scrollEnabled: false);
    }
}

[EditorModule]
public class EventSpeakerSelectorEditorModule : BaseSelectorEditorModule<IEventSpeakerService>
{
    public const string Id = "event_speaker_selector";
    public override string UniqueId => Id;
    public override string ListName => "Event Speaker";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<EventSpeakerViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel((EventSpeakerId)id, _service.Retrieve(id)));
    }
}

[EditorModule]
public class ItemSelectorEditorModule : BaseSelectorEditorModule<IItemService>
{
    public const string Id = "item_selector";
    public override string UniqueId => Id;
    public override string ListName => "Item";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<ItemViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel((ItemId)id, _service.Retrieve(id)));
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

        _viewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<BuildingViewModel>(),
            _service,
            command =>
            {
                var lst = new List<IMiniViewModel>();
                foreach (var kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
                {
                    lst.Add(new BuildingSimpleKingdomMiniViewModel(sp, kingdom));
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
        _viewModel = _selectorVmFactory.CreateWorkspace(
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
    public override string ListName => "Gimmick Object";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<GimmickObjectViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel((GimmickObjectId)id, _service.Retrieve(id)));
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
        _viewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<EpisodeViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new EpisodeMiniViewModel(sp, bw, sw, sk, ms, _service.Retrieve(id), id, command)).ToList()
            );
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
        _viewModel = _selectorVmFactory.CreateWorkspace(
            modServices.Get<KingdomViewModel>(),
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new KingdomMiniViewModel(sp, _service.Retrieve(id), id, command)).ToList()
            );
    }
}

[EditorModule]
public class BattleConfigSelectorEditorModule : BaseSelectorEditorModule<IBattleConfigService>
{
    public const string Id = "battle_config_selector";
    public override string UniqueId => Id;
    public override string ListName => "Battle Config";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<BattleConfigViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel((BattleConfigId)id, _service.Retrieve(id)));
    }
}

[EditorModule]
public class GimmickRangeSelectorEditorModule : BaseSelectorEditorModule<IGimmickRangeService>
{
    public const string Id = "gimmick_range_selector";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick Range";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<MoveRangeViewModel>();
        _viewModel = _selectorVmFactory.Create(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

//public class MoveAnimationGridEditorModule : BaseSelectorEditorModule<IAbilityService>
//{
//    public const string Id = "move_animation_grid";
//    public override string UniqueId => Id;
//    public override string ListName => "Move Animation (Grid)";
//    public override void Initialise(IServiceGetter modServices)
//    {
//        base.Initialise(modServices);
//        var vm = modServices.Get<IAbilityViewModel>();
//        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((AbilityId)id, _service.Retrieve(id)));
//    }
//}


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
public class MapSelectorEditorModule : EditorModule, ISelectableModule
{
    public const string Id = "map_selector";
    public override string UniqueId => Id;
    public override string ListName => "Map";
    private SelectorViewModel? _viewModel;
    public override object? ViewModel => _viewModel;
    private IMapService? _service;
    private MapId? _currentId;
    private PSLM? _currentMap;
    private MapViewModel? _nestedVm;

    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IMapService>();
        _nestedVm = modServices.Get<MapViewModel>();
        var selectorVmFactory = modServices.Get<ISelectorViewModelFactory>();
        if (_nestedVm == null)
        {
            throw new Exception("Failed to resolve map view model");
        }
        _currentMap = null;
        _currentId = null;
        var mapComboItems = _service.GetMapIds().Select(i => new SelectorComboBoxItem((int)i, i.ToString())).ToList();
        _viewModel = selectorVmFactory.Create(null, mapComboItems, _nestedVm, id =>
        {
            if (_currentMap != null && _currentId != null)
            {
                _service.Save(_currentId.Value, _currentMap);
            }
            var mid = (MapId)id;
            _currentMap = _service.Retrieve(mid);
            _currentId = mid;
            _nestedVm.SetModel(mid, _currentMap);
        },
        id => _service.GetMapIds().Select(i => (int)i).Contains(id),
        scrollEnabled: false
        );

        _nestedVm.RequestSave += NestedVm_RequestSave;
        _nestedVm.RequestReload += NestedVm_RequestReload;
    }

    private void NestedVm_RequestReload(object? sender, System.EventArgs e)
    {
        if (_viewModel == null || _service == null || _nestedVm == null)
        {
            return;
        }
        var id = (MapId)_viewModel.Selected;
        _currentMap = _service.Retrieve(id);
        _nestedVm.SetModel(id, _currentMap);
    }

    private void NestedVm_RequestSave(object? sender, System.EventArgs e)
    {
        if (_viewModel == null || _service == null || _currentMap == null)
        {
            return;
        }
        _service.Save((MapId)_viewModel.Selected, _currentMap);
    }

    public override void OnPageClosing()
    {
        if (_viewModel == null || _service == null || _nestedVm == null)
        {
            return;
        }
        if (_currentMap != null)
        {
            _service.Save((MapId)_viewModel.Selected, _currentMap);
        }
    }

    public override void OnPatchingRom()
    {
        if (_viewModel == null || _service == null)
        {
            return;
        }
        if (_currentMap != null)
        {
            _service.Save((MapId)_viewModel.Selected, _currentMap);
        }
    }

    public override void Deactivate()
    {
        if (_viewModel == null || _service == null)
        {
            return;
        }
        if (_currentMap != null)
        {
            _service.Save((MapId)_viewModel.Selected, _currentMap);
        }
    }

    public void Select(int selectId)
    {
        if (_viewModel == null)
        {
            return;
        }
        _viewModel.Selected = selectId;
    }
}
