#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Windows.ViewModels;

[EditorModule]
public class PokemonSelectorEditorModule : BaseSelectorEditorModule<IPokemonService>
{
    public const string Id = "pokemon_selector";
    public override string UniqueId => Id;
    public override string ListName => "Pokemon";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<PokemonViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((PokemonId)id, _service.Retrieve(id)));
    }
}

//public class PokemonGridEditorModule : IEditorModule
//{
//    public const string Id = "pokemon_grid";
//    public string UniqueId => Id;
//    public string ListName => "Pokemon (Grid)";
//    public override void Initialise(IServiceGetter modServices)
//    {
//        base.Initialise(modServices);
//        var _service = context.DataService.Pokemon;
//        object vmFactory(int id, object model) => new PokemonGridItemViewModel((PokemonId)id, (Pokemon)model);
//        _viewModel = new GridViewModel(container, _service, vmFactory);
//    }
//}

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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((AbilityId)id, _service.Retrieve(id)));
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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((WarriorSkillId)id, _service.Retrieve(id)));
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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

[EditorModule]
public class MoveSelectorEditorModule : BaseSelectorEditorModule<IMoveService>
{
    public const string Id = "move_selector";
    public override string UniqueId => Id;
    public override string ListName => "Move";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<MoveViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((MoveId)id, _service.Retrieve(id)));
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
public class BaseWarriorSelectorEditorModule : BaseSelectorEditorModule<IBaseWarriorService>
{
    public const string Id = "base_warrior_selector";
    public override string UniqueId => Id;
    public override string ListName => "Base Warrior";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<BaseWarriorViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((WarriorId)id, _service.Retrieve(id)));
    }
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
        _viewModel = new SelectorViewModelWithoutScroll(items, vm, id => vm.SetModel(id, _service.Retrieve(id)), _service.ValidateId);
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
        _viewModel = new SelectorViewModelWithoutScroll(items, vm, id => vm.SetModel(id, _service), pokemonService.ValidateId);
    }
}

[EditorModule]
public class ScenarioWarriorWorkspaceEditorModule : BaseSelectorEditorModule<IScenarioWarriorService>
{
    public const string Id = "scenario_warrior_workspace";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Warrior";

    private IScenarioPokemonService? _scenarioPokemonService;
    private IScenarioKingdomService? _scenarioKingdomService;

    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _scenarioPokemonService = modServices.Get<IScenarioPokemonService>();
        _scenarioKingdomService = modServices.Get<IScenarioKingdomService>();
        var spVm = modServices.Get<ScenarioPokemonViewModel.Factory>()();
        var vm = modServices.Get<ScenarioWarriorWorkspaceViewModel>().Init(spVm);
        var spService = modServices.Get<IScenarioPokemonService>();

        _viewModel = new SelectorViewModelWithoutScroll(_service, vm,
            id => vm.SetModel((ScenarioId)id, _service.Retrieve(id), spService.Retrieve(id)));
    }

    public override void OnPatchingRom()
    {
        base.OnPatchingRom();
        _scenarioPokemonService?.Save();
        _scenarioKingdomService?.Save();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _scenarioPokemonService?.Save();
        _scenarioKingdomService?.Save();
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
        _viewModel = new SelectorViewModelWithoutScroll(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

[EditorModule]
public class ScenarioBuildingSelectorEditorModule : BaseSelectorEditorModule<IScenarioBuildingService>
{
    public const string Id = "scenario_building_selector";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Building";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<ScenarioBuildingViewModel>();
        _viewModel = new SelectorViewModelWithoutScroll(_service, vm, id => vm.SetModel(id, _service.Retrieve(id)));
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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((ItemId)id, _service.Retrieve(id)));
    }
}

[EditorModule]
public class BuildingSelectorEditorModule : BaseSelectorEditorModule<IBuildingService>
{
    public const string Id = "building_selector";
    public override string UniqueId => Id;
    public override string ListName => "Building";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<BuildingViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((BuildingId)id, _service.Retrieve(id)));
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
public class GimmickSelectorEditorModule : BaseSelectorEditorModule<IGimmickService>
{
    public const string Id = "gimmick_selector";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<GimmickViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((GimmickId)id, _service.Retrieve(id)));
    }
}

[EditorModule]
public class EpisodeSelectorEditorModule : BaseSelectorEditorModule<IEpisodeService>
{
    public const string Id = "episode_selector";
    public override string UniqueId => Id;
    public override string ListName => "Episode";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<EpisodeViewModel>();
        var msgService = modServices.Get<ICachedMsgBlockService>();
        var episodeComboItems = _service
            .ValidIds()
            .Select(i => new SelectorComboBoxItem(i, msgService.GetMsgOfType(MsgShortcut.EpisodeName, i)))
            .ToList();
        
        _viewModel = new SelectorViewModel(episodeComboItems, vm, id => vm.SetModel((EpisodeId)id, _service.Retrieve(id)), _service.ValidateId);
    }
}

[EditorModule]
public class KingdomSelectorEditorModule : BaseSelectorEditorModule<IKingdomService>
{
    public const string Id = "kingdom_selector";
    public override string UniqueId => Id;
    public override string ListName => "Kingdom";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<KingdomViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((KingdomId)id, _service.Retrieve(id)));
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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((BattleConfigId)id, _service.Retrieve(id)));
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
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
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
public class MapSelectorEditorModule : EditorModule
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
        if (_nestedVm == null)
        {
            throw new Exception("Failed to resolve map view model");
        }
        _currentMap = null;
        _currentId = null;
        var mapComboItems = _service.GetMapIds().Select(i => new SelectorComboBoxItem((int)i, i.ToString())).ToList();
        _viewModel = new SelectorViewModelWithoutScroll(mapComboItems, _nestedVm, id =>
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
        id => _service.GetMapIds().Select(i => (int)i).Contains(id));

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