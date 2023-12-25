using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.Windows.Services.Concrete;
using RanseiLink.Windows.ViewModels;
using DryIoc;
using RanseiLink.PluginModule.Services;
using RanseiLink.Core.Settings;
using RanseiLink.Windows.Dialogs;
using System.Reflection;
using RanseiLink.Windows.Services;
using RanseiLink.Core;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.DragDrop;

namespace RanseiLink.Windows;

public class WpfServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.RegisterInstance(CreateDialogLocator());
        builder.Register<IDialogService, DialogService>(Reuse.Singleton);
        builder.Register<IAsyncDialogService, WpfAsyncDialogService>(Reuse.Singleton);
        builder.Register<IFolderDropHandler, FolderDropHandler>(Reuse.Singleton);
        builder.Register<IFileDropHandlerFactory, FileDropHandlerFactory>(Reuse.Singleton);

        builder.Register<IPluginService, PluginService>(Reuse.Singleton);
        builder.Register<IThemeService, ThemeService>(Reuse.Singleton);
        builder.Register<IExternalService, ExternalService>(Reuse.Singleton);
        builder.Register<IFallbackSpriteManager, FallbackSpriteManager>(Reuse.Singleton);

        builder.Register<MainWindowViewModel>(Reuse.Singleton);
        builder.Register<IModSelectionViewModel, ModSelectionViewModel>(Reuse.Singleton);
        builder.Register<IMainEditorViewModel, MainEditorViewModel>(Reuse.Singleton);

        builder.RegisterDelegate(context =>
            new ModListItemViewModelFactory((parent, mod) =>
                new ModListItemViewModel(
                    parent,
                    mod,
                    context.Resolve<IModManager>(),
                    context.Resolve<IModPatchingService>(),
                    context.Resolve<IDialogService>(),
                    context.Resolve<ISettingService>(),
                    context.Resolve<IPluginLoader>(),
                    context.Resolve<IModServiceGetterFactory>(),
                    context.Resolve<IFileDropHandlerFactory>(),
                    context.Resolve<IFolderDropHandler>()
            )), Reuse.Singleton);

        builder.Register<IJumpService, JumpService>(Reuse.Singleton);

        // editor modules
        foreach (var type in GetType().Assembly.GetTypes())
        {
            if (typeof(EditorModule).IsAssignableFrom(type) && !type.IsAbstract && type.GetCustomAttribute<EditorModuleAttribute>() != null)
            {
                builder.Register(typeof(EditorModule), type);
            }
        }
    }

    private static IDialogLocator CreateDialogLocator()
    {
        var locator = new RegistryDialogLocator();

        locator.Register<ImageListDialog, ImageListViewModel>();
        locator.Register<ModCommitDialog, ModCommitViewModel>();
        locator.Register<ModCreateBasedOnDialog, ModCreateBasedOnViewModel>();
        locator.Register<ModCreationDialog, ModCreationViewModel>();
        locator.Register<ModDeleteDialog, ModDeleteViewModel>();
        locator.Register<ModEditInfoDialog, ModEditInfoViewModel>();
        locator.Register<ModExportDialog, ModExportViewModel>();
        locator.Register<ModifyMapDimensionsDialog, ModifyMapDimensionsViewModel>();
        locator.Register<ModImportDialog, ModImportViewModel>();
        locator.Register<ModUpgradeDialog, ModUpgradeViewModel>();
        locator.Register<PopulateDefaultSpriteDialog, PopulateDefaultSpriteViewModel>();
        locator.Register<SimplifyPaletteDialog, SimplifyPaletteViewModel>();
        locator.Register<AnimExportDialog, AnimExportViewModel>();

        return locator;
    }
}

public class WpfModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<IIdToNameService, IdToNameService>(Reuse.Singleton);
        builder.Register<ISpriteManager, SpriteManager>(Reuse.Singleton);
        builder.Register<ICachedSpriteProvider, CachedSpriteProvider>(Reuse.Singleton);
        builder.Register<IMapManager, MapManager>(Reuse.Singleton);
        builder.Register<IAnimGuiManager, AnimGuiManager>(Reuse.Singleton);

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
        builder.Register<MaxLinkWarriorViewModel>();
        builder.Register<MaxLinkPokemonViewModel>();
        builder.Register<MoveRangeViewModel>();
        builder.Register<MoveViewModel>();
        builder.Register<PokemonViewModel>();
        builder.Register<ScenarioAppearPokemonViewModel>();
        builder.Register<ScenarioBuildingViewModel>();
        builder.Register<ScenarioPokemonViewModel>();
        builder.Register<SpriteTypeViewModel>();
        builder.Register<WarriorNameTableViewModel>();
        builder.Register<WarriorSkillViewModel>();
        builder.Register<ScenarioWarriorWorkspaceViewModel>();

        builder.Register<MsgGridViewModel>();
        builder.Register<ScenarioWarriorGridViewModel>();

        builder.Register<SpriteItemViewModel>();
        builder.Register<SwMiniViewModel>();
        builder.Register<SwKingdomMiniViewModel>();
        builder.Register<SwSimpleKingdomMiniViewModel>();

        builder.RegisterDelegate(context => new ScenarioPokemonViewModel.Factory(() => context.Resolve<ScenarioPokemonViewModel>()), Reuse.Singleton);
        builder.RegisterDelegate(context => new SpriteItemViewModel.Factory(() => context.Resolve<SpriteItemViewModel>()), Reuse.Singleton);
        builder.RegisterDelegate(context => new SwMiniViewModel.Factory(() => context.Resolve<SwMiniViewModel>()), Reuse.Singleton);
        builder.RegisterDelegate(context => new SwKingdomMiniViewModel.Factory(() => context.Resolve<SwKingdomMiniViewModel>()), Reuse.Singleton);
        builder.RegisterDelegate(context => new SwSimpleKingdomMiniViewModel.Factory(() => context.Resolve<SwSimpleKingdomMiniViewModel>()), Reuse.Singleton);

        builder.Register<BannerViewModel>();
    }
}