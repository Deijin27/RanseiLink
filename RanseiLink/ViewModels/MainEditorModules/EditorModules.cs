using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class PokemonSelectorEditorModule : BaseSelectorEditorModule<IPokemonService>
{
    public const string Id = "pokemon_selector";
    public override string UniqueId => Id;
    public override string ListName => "Pokemon";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IPokemonViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
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

public class AbilitySelectorEditorModule : BaseSelectorEditorModule<IAbilityService>
{
    public const string Id = "ability_selector";
    public override string UniqueId => Id;
    public override string ListName => "Ability";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IAbilityViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((AbilityId)id, _service.Retrieve(id)));
    }
}

public class WarriorSkillSelectorEditorModule : BaseSelectorEditorModule<IWarriorSkillService>
{
    public const string Id = "warrior_skill_selector";
    public override string UniqueId => Id;
    public override string ListName => "Warrior Skill";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IWarriorSkillViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((WarriorSkillId)id, _service.Retrieve(id)));
    }
}

public class MoveRangeSelectorEditorModule : BaseSelectorEditorModule<IMoveRangeService>
{
    public const string Id = "move_range_selector";
    public override string UniqueId => Id;
    public override string ListName => "Move Range";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IMoveRangeViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

public class MoveSelectorEditorModule : BaseSelectorEditorModule<IMoveService>
{
    public const string Id = "move_selector";
    public override string UniqueId => Id;
    public override string ListName => "Move";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IMoveViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((MoveId)id, _service.Retrieve(id)));
    }
}

public class WarriorNameTableEditorModule : EditorModule
{
    public const string Id = "warrior_name_table";
    public override string UniqueId => Id;
    public override string ListName => "Warrior Name Table";
    public override object ViewModel => _viewModel;

    private WarriorNameTableViewModel _viewModel;
    private IBaseWarriorService _service;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IBaseWarriorService>();
        _viewModel = new WarriorNameTableViewModel();
        _viewModel.SetModel(_service.NameTable);
    }
    public override void Deactivate() => _service.Save();
    public override void OnPatchingRom() => _service.Save();
    public override void OnPluginComplete() => _viewModel.SetModel(_service.NameTable);
}

public class BaseWarriorSelectorEditorModule : BaseSelectorEditorModule<IBaseWarriorService>
{
    public const string Id = "base_warrior_selector";
    public override string UniqueId => Id;
    public override string ListName => "Base Warrior";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IBaseWarriorViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

public class MaxLinkSelectorEditorModule : BaseSelectorEditorModule<IMaxLinkService>
{
    public const string Id = "max_link_selector";
    public override string UniqueId => Id;
    public override string ListName => "Max Link";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IMaxLinkViewModel>();
        _viewModel = new SelectorViewModelWithoutScroll(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

public class ScenarioWarriorGridSelectorEditorModule : BaseSelectorEditorModule<IScenarioWarriorService>
{
    public const string Id = "scenario_warrior_grid";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Warrior (Grid)";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IScenarioWarriorGridViewModel>();
        _viewModel = new SelectorViewModelWithoutScroll(_service, vm, id => vm.SetModel(id, _service.Retrieve(id)));
    }
}

public class ScenarioWarriorSelectorEditorModule : BaseSelectorEditorModule<IScenarioWarriorService>
{
    public const string Id = "scenario_warrior_selector";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Warrior";
    private IChildScenarioWarriorService _childScenarioWarriorService;
    private IScenarioPokemonService _scenarioPokemonService;
    private int _scenario;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _scenarioPokemonService = modServices.Get<IScenarioPokemonService>();
        var vm = modServices.Get<IScenarioWarriorViewModel>();
        _childScenarioWarriorService = _service.Retrieve(0);
        var innerSelector = new SelectorViewModel(_childScenarioWarriorService, vm, id => vm.SetModel((ScenarioId)_scenario, id, _childScenarioWarriorService.Retrieve(id)));
        _viewModel = new SelectorViewModelWithoutScroll(_service, innerSelector, id =>
        {
            _scenario = id;
            _childScenarioWarriorService = _service.Retrieve(id);
            innerSelector.SetDisplayItems(_childScenarioWarriorService.GetComboBoxItemsExceptDefault());
            vm.SetModel((ScenarioId)_scenario, innerSelector.Selected, _childScenarioWarriorService.Retrieve(innerSelector.Selected));
        });
    }

    public override void OnPatchingRom()
    {
        base.OnPatchingRom();
        _scenarioPokemonService?.Save();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _scenarioPokemonService?.Save();
    }
}

public class ScenarioPokemonSelectorEditorModule : BaseSelectorEditorModule<IScenarioPokemonService>
{
    public const string Id = "scenario_pokemon_selector";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Pokemon";
    private IChildScenarioPokemonService _childScenarioPokemonService;
    private int _scenario;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IScenarioPokemonViewModel>();
        _childScenarioPokemonService = _service.Retrieve(0);
        var innerSelector = new SelectorViewModel(_childScenarioPokemonService, vm, id => vm.SetModel((ScenarioId)_scenario, id, _childScenarioPokemonService.Retrieve(id)));
        _viewModel = new SelectorViewModelWithoutScroll(_service, innerSelector, id =>
        {
            _scenario = id;
            _childScenarioPokemonService = _service.Retrieve(id);
            innerSelector.SetDisplayItems(_childScenarioPokemonService.GetComboBoxItemsExceptDefault());
            vm.SetModel((ScenarioId)_scenario, innerSelector.Selected, _childScenarioPokemonService.Retrieve(innerSelector.Selected));
        });
    }
}

public class ScenarioAppearPokemonSelectorEditorModule : BaseSelectorEditorModule<IScenarioAppearPokemonService>
{
    public const string Id = "scenario_appear_pokemon_selector";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Appear Pokemon";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IScenarioAppearPokemonViewModel>();
        _viewModel = new SelectorViewModelWithoutScroll(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

public class ScenarioKingdomSelectorEditorModule : BaseSelectorEditorModule<IScenarioKingdomService>
{
    public const string Id = "scenario_kingdom_selector";
    public override string UniqueId => Id;
    public override string ListName => "Scenario Kingdom";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IScenarioKingdomViewModel>();
        _viewModel = new SelectorViewModelWithoutScroll(_service, vm, id => vm.SetModel(id, _service.Retrieve(id)));
    }
}

public class EventSpeakerSelectorEditorModule : BaseSelectorEditorModule<IEventSpeakerService>
{
    public const string Id = "event_speaker_selector";
    public override string UniqueId => Id;
    public override string ListName => "Event Speaker";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IEventSpeakerViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel(_service.Retrieve(id)));
    }
}

public class ItemSelectorEditorModule : BaseSelectorEditorModule<IItemService>
{
    public const string Id = "item_selector";
    public override string UniqueId => Id;
    public override string ListName => "Item";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IItemViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((ItemId)id, _service.Retrieve(id)));
    }
}

public class BuildingSelectorEditorModule : BaseSelectorEditorModule<IBuildingService>
{
    public const string Id = "building_selector";
    public override string UniqueId => Id;
    public override string ListName => "Building";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IBuildingViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((BuildingId)id, _service.Retrieve(id)));
    }
}

public class MsgGridEditorModule : EditorModule
{
    public const string Id = "msg_grid";
    public override string UniqueId => Id;
    public override string ListName => "Text";
    public override object ViewModel => _viewModel;
    private MsgGridViewModel _viewModel;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _viewModel = modServices.Get<MsgGridViewModel>();
    }
    public override void OnPluginComplete() => _viewModel.SearchCommand.Execute(null);
}

public class GimmickSelectorEditorModule : BaseSelectorEditorModule<IGimmickService>
{
    public const string Id = "gimmick_selector";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IGimmickViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((GimmickId)id, _service.Retrieve(id)));
    }
}

public class EpisodeSelectorEditorModule : BaseSelectorEditorModule<IEpisodeService>
{
    public const string Id = "episode_selector";
    public override string UniqueId => Id;
    public override string ListName => "Episode";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IEpisodeViewModel>();
        var msgService = modServices.Get<ICachedMsgBlockService>();
        var episodeComboItems = _service
            .ValidIds()
            .Select(i => new SelectorComboBoxItem(i, msgService.GetMsgOfType(MsgShortcut.EpisodeName, i)))
            .ToList();
        
        _viewModel = new SelectorViewModel(episodeComboItems, vm, id => vm.SetModel((EpisodeId)id, _service.Retrieve(id)), _service.ValidateId);
    }
}

public class KingdomSelectorEditorModule : BaseSelectorEditorModule<IKingdomService>
{
    public const string Id = "kingdom_selector";
    public override string UniqueId => Id;
    public override string ListName => "Kingdom";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IKingdomViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((KingdomId)id, _service.Retrieve(id)));
    }
}

public class BattleConfigSelectorEditorModule : BaseSelectorEditorModule<IBattleConfigService>
{
    public const string Id = "battle_config_selector";
    public override string UniqueId => Id;
    public override string ListName => "Battle Config";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IBattleConfigViewModel>();
        _viewModel = new SelectorViewModel(_service, vm, id => vm.SetModel((BattleConfigId)id, _service.Retrieve(id)));
    }
}

public class GimmickRangeSelectorEditorModule : BaseSelectorEditorModule<IGimmickRangeService>
{
    public const string Id = "gimmick_range_selector";
    public override string UniqueId => Id;
    public override string ListName => "Gimmick Range";
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        var vm = modServices.Get<IMoveRangeViewModel>();
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

public class MapSelectorEditorModule : EditorModule
{
    public const string Id = "map_selector";
    public override string UniqueId => Id;
    public override string ListName => "Map";
    private SelectorViewModel _viewModel;
    public override object ViewModel => _viewModel;
    private IMapService _service;
    private MapId? _currentId;
    private PSLM _currentMap;
    private IMapViewModel _nestedVm;

    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IMapService>();
        _nestedVm = modServices.Get<IMapViewModel>();
        var mapComboItems = _service.GetMapIds().Select(i => new SelectorComboBoxItem((int)i, i.ToString())).ToList();
        _viewModel = new SelectorViewModelWithoutScroll(mapComboItems, _nestedVm, id =>
        {
            if (_currentMap != null && _currentId != null)
            {
                _service.Save(_currentId.Value, _currentMap);
            }
            _currentMap = _service.Retrieve((MapId)id);
            _currentId = (MapId)id;
            _nestedVm.SetModel(_currentMap);
        },
        id => _service.GetMapIds().Select(i => (int)i).Contains(id));
    }

    public override void OnPageClosing()
    {
        if (_currentMap != null)
        {
            _service.Save((MapId)_viewModel.Selected, _currentMap);
        }
    }

    public override void OnPatchingRom()
    {
        if (_currentMap != null)
        {
            _service.Save((MapId)_viewModel.Selected, _currentMap);
        }
    }
    public override void OnPluginComplete()
    {
        if (_currentMap != null)
        {
            _nestedVm.SetModel(_currentMap);
        }
    }

    public override void Deactivate()
    {
        if (_currentMap != null)
        {
            _service.Save((MapId)_viewModel.Selected, _currentMap);
        }
    }
}

public class SpriteEditorModule : EditorModule
{
    public const string Id = "sprites";
    public override string UniqueId => Id;
    public override string ListName => "Sprites";
    public override object ViewModel => _viewModel;

    private SpriteTypeViewModel _viewModel;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _viewModel = modServices.Get<SpriteTypeViewModel>();
    }
}

public class BannerEditorModule : EditorModule
{
    public const string Id = "banner";
    public override string UniqueId => Id;
    public override string ListName => "Banner";
    public override object ViewModel => _viewModel;

    private BannerViewModel _viewModel;
    private IBannerService _service;
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<IBannerService>();
        _viewModel = modServices.Get<BannerViewModel>();
    }
    public override void Deactivate() => _service.Save();
    public override void OnPatchingRom() => _service.Save();
}