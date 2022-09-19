using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.Services.Concrete;
using RanseiLink.ViewModels;
using DryIoc;
using RanseiLink.PluginModule.Services;

namespace RanseiLink.Services;

public class WpfServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IDialogService, DialogService>(Reuse.Singleton);
        builder.Register<IPluginService, PluginService>(Reuse.Singleton);
        builder.Register<IThemeService, ThemeService>(Reuse.Singleton);
        builder.Register<IExternalService, ExternalService>(Reuse.Singleton);

        builder.Register<MainWindowViewModel>(Reuse.Singleton);
        builder.Register<IModSelectionViewModel, ModSelectionViewModel>(Reuse.Singleton);
        builder.Register<IMainEditorViewModel, MainEditorViewModel>(Reuse.Singleton);

        ContainerProvider.ModServiceGetterFactory.AddModule(new WpfModServiceModule());

        builder.RegisterDelegate(context =>
            new ModListItemViewModelFactory((parent, mod) =>
                new ModListItemViewModel(
                    parent,
                    mod,
                    context.Resolve<IModManager>(),
                    context.Resolve<IModPatchingService>(),
                    context.Resolve<IDialogService>(),
                    context.Resolve<IPluginLoader>(),
                    context.Resolve<IModServiceGetterFactory>()
            )), Reuse.Singleton);

        builder.Register<IJumpService, JumpService>(Reuse.Singleton);

        // editor modules
        foreach (var type in GetType().Assembly.GetTypes())
        {
            if (typeof(EditorModule).IsAssignableFrom(type) && !type.IsAbstract)
            {
                builder.Register(typeof(EditorModule), type);
            }
        }
    }
}

public class WpfModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<ICachedMsgBlockService, CachedMsgBlockService>(Reuse.Singleton);
        builder.Register<IIdToNameService, IdToNameService>(Reuse.Singleton);
        builder.Register<ISpriteManager, SpriteManager>(Reuse.Singleton);
        builder.Register<IMapManager, MapManager>(Reuse.Singleton);

        builder.Register<AbilityViewModel>();
        builder.Register<BaseWarriorViewModel>();
        builder.Register<BattleConfigViewModel>();
        builder.Register<BuildingViewModel>();
        builder.Register<EpisodeViewModel>();
        builder.Register<EventSpeakerViewModel>();
        builder.Register<GimmickViewModel>();
        builder.Register<ItemViewModel>();
        builder.Register<KingdomViewModel>();
        builder.Register<MapViewModel>();
        builder.Register<MaxLinkViewModel>();
        builder.Register<MoveRangeViewModel>();
        builder.Register<MoveViewModel>();
        builder.Register<PokemonViewModel>();
        builder.Register<ScenarioAppearPokemonViewModel>();
        builder.Register<ScenarioKingdomViewModel>();
        builder.Register<ScenarioPokemonViewModel>();
        builder.Register<ScenarioWarriorViewModel>();
        builder.Register<SpriteTypeViewModel>();
        builder.Register<WarriorNameTableViewModel>();
        builder.Register<WarriorSkillViewModel>();

        builder.Register<MsgGridViewModel>();
        builder.Register<ScenarioWarriorGridViewModel>();

        builder.RegisterDelegate(context =>
            new SpriteItemViewModelFactory(file =>
                new SpriteItemViewModel(
                    file,
                    context.Resolve<ISpriteManager>(),
                    context.Resolve<IOverrideDataProvider>(),
                    context.Resolve<IDialogService>()
            )));

        builder.Register<BannerViewModel>();
    }
}