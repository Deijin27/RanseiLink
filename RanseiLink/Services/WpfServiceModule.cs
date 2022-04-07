using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.Services.Concrete;
using RanseiLink.ViewModels;
using Autofac;

namespace RanseiLink.Services;

public class WpfServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
        builder.RegisterType<PluginService>().As<IPluginService>().SingleInstance();
        builder.RegisterType<ThemeService>().As<IThemeService>().SingleInstance();
        builder.RegisterType<ExternalService>().As<IExternalService>().SingleInstance();

        builder.RegisterType<MainWindowViewModel>().As<MainWindowViewModel>().SingleInstance();
        builder.RegisterType<ModSelectionViewModel>().As<ModSelectionViewModel>().SingleInstance();
        builder.RegisterType<MainEditorViewModel>().As<MainEditorViewModel>().SingleInstance();

        ContainerProvider.ModServiceGetterFactory.AddModule(new WpfModServiceModule());

        builder.RegisterType<ModListItemViewModel>().As<ModListItemViewModel>();
        builder.RegisterType<ModListItemViewModelFactory>().As<IModListItemViewModelFactory>().SingleInstance();

        builder.RegisterType<JumpService>().As<IJumpService>().SingleInstance();
    }
}

public class WpfModServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CachedMsgBlockService>().As<ICachedMsgBlockService>().SingleInstance();
        builder.RegisterType<IdToNameService>().As<IIdToNameService>();

        builder.RegisterType<AbilityViewModel>().As<IAbilityViewModel>();
        builder.RegisterType<BaseWarriorViewModel>().As<IBaseWarriorViewModel>();
        builder.RegisterType<BattleConfigViewModel>().As<IBattleConfigViewModel>();
        builder.RegisterType<BuildingViewModel>().As<IBuildingViewModel>();
        builder.RegisterType<EventSpeakerViewModel>().As<IEventSpeakerViewModel>();
        builder.RegisterType<GimmickViewModel>().As<IGimmickViewModel>();
        builder.RegisterType<ItemViewModel>().As<IItemViewModel>();
        builder.RegisterType<KingdomViewModel>().As<IKingdomViewModel>();
        builder.RegisterType<MapViewModel>().As<IMapViewModel>();
        builder.RegisterType<MaxLinkViewModel>().As<IMaxLinkViewModel>();
        builder.RegisterType<MoveRangeViewModel>().As<IMoveRangeViewModel>();
        builder.RegisterType<MoveViewModel>().As<IMoveViewModel>();
        builder.RegisterType<PokemonViewModel>().As<IPokemonViewModel>();
        builder.RegisterType<ScenarioAppearPokemonViewModel>().As<IScenarioAppearPokemonViewModel>();
        builder.RegisterType<ScenarioKingdomViewModel>().As<IScenarioKingdomViewModel>();
        builder.RegisterType<ScenarioPokemonViewModel>().As<IScenarioPokemonViewModel>();
        builder.RegisterType<ScenarioWarriorViewModel>().As<IScenarioWarriorViewModel>();
        builder.RegisterType<SpriteTypeViewModel>().As<SpriteTypeViewModel>();
        builder.RegisterType<WarriorNameTableViewModel>().As<IWarriorNameTableViewModel>();
        builder.RegisterType<WarriorSkillViewModel>().As<IWarriorSkillViewModel>();

        builder.RegisterType<MsgGridViewModel>().As<MsgGridViewModel>();
    }
}