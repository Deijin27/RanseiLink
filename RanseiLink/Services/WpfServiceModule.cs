using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.Services.Concrete;
using RanseiLink.ViewModels;
using Autofac;
using System.Collections.Generic;
using System;
using System.Linq;

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
        builder.RegisterType<ModSelectionViewModel>().As<IModSelectionViewModel>().SingleInstance();
        builder.RegisterType<MainEditorViewModel>().As<IMainEditorViewModel>().SingleInstance();

        ContainerProvider.ModServiceGetterFactory.AddModule(new WpfModServiceModule());

        builder.RegisterType<ModListItemViewModel>().As<IModListItemViewModel>();
        builder.RegisterType<ModListItemViewModelFactory>().As<IModListItemViewModelFactory>().SingleInstance();

        builder.RegisterType<JumpService>().As<IJumpService>().SingleInstance();

        // editor modules
        IEnumerable<Type> types = ThisAssembly
                .GetTypes()
                .Where(i => typeof(EditorModule).IsAssignableFrom(i) && !i.IsAbstract);
        foreach (Type t in types)
        {
            builder.RegisterType(t).As<EditorModule>();
        }
    }
}

public class WpfModServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CachedMsgBlockService>().As<ICachedMsgBlockService>().SingleInstance();
        builder.RegisterType<IdToNameService>().As<IIdToNameService>().SingleInstance();
        builder.RegisterType<SpriteManager>().As<ISpriteManager>().SingleInstance();
        builder.RegisterType<MapManager>().As<IMapManager>().SingleInstance();

        builder.RegisterType<AbilityViewModel>().AsSelf();
        builder.RegisterType<BaseWarriorViewModel>().AsSelf();
        builder.RegisterType<BattleConfigViewModel>().AsSelf();
        builder.RegisterType<BuildingViewModel>().AsSelf();
        builder.RegisterType<EpisodeViewModel>().AsSelf();
        builder.RegisterType<EventSpeakerViewModel>().AsSelf();
        builder.RegisterType<GimmickViewModel>().AsSelf();
        builder.RegisterType<ItemViewModel>().AsSelf();
        builder.RegisterType<KingdomViewModel>().AsSelf();
        builder.RegisterType<MapViewModel>().AsSelf();
        builder.RegisterType<MaxLinkViewModel>().AsSelf();
        builder.RegisterType<MoveRangeViewModel>().AsSelf();
        builder.RegisterType<MoveViewModel>().AsSelf();
        builder.RegisterType<PokemonViewModel>().AsSelf();
        builder.RegisterType<ScenarioAppearPokemonViewModel>().AsSelf();
        builder.RegisterType<ScenarioKingdomViewModel>().AsSelf();
        builder.RegisterType<ScenarioPokemonViewModel>().AsSelf();
        builder.RegisterType<ScenarioWarriorViewModel>().AsSelf();
        builder.RegisterType<SpriteTypeViewModel>().AsSelf();
        builder.RegisterType<WarriorNameTableViewModel>().AsSelf();
        builder.RegisterType<WarriorSkillViewModel>().AsSelf();

        builder.RegisterType<MsgGridViewModel>().As<MsgGridViewModel>();
        builder.RegisterType<ScenarioWarriorGridViewModel>().As<IScenarioWarriorGridViewModel>();

        builder.RegisterType<SpriteItemViewModel>().As<SpriteItemViewModel>();

        builder.RegisterType<BannerViewModel>().As<BannerViewModel>();
    }
}