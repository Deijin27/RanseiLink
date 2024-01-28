using RanseiLink.PluginModule.Api;
using RanseiLink.Windows.Services.Concrete;
using RanseiLink.Windows.ViewModels;
using DryIoc;
using RanseiLink.Windows.Dialogs;
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
        builder.Register<IDispatcherService, DispatcherService>(Reuse.Singleton);
        builder.Register<IClipboardService, ClipboardService>(Reuse.Singleton);
        builder.Register<IAppInfoService, AppInfoService>(Reuse.Singleton);
        builder.Register<IAsyncDialogService, WpfAsyncDialogService>(Reuse.Singleton);
        builder.Register<IFolderDropHandler, FolderDropHandler>(Reuse.Singleton);
        builder.Register<IFileDropHandlerFactory, FileDropHandlerFactory>(Reuse.Singleton);
        builder.Register<IPathToImageConverter, PathToImageConverter>(Reuse.Singleton);

        builder.Register<IAsyncPluginService, AsyncPluginService>(Reuse.Singleton);
        builder.Register<IThemeService, ThemeService>(Reuse.Singleton);

        builder.Register<IJumpService, JumpService>(Reuse.Singleton);
        builder.Register<ISelectorViewModelFactory, SelectorViewModelFactory>(Reuse.Singleton);


        builder.Register<EditorModule, PokemonSelectorEditorModule>();
        builder.Register<EditorModule, AbilitySelectorEditorModule>();
        builder.Register<EditorModule, WarriorSkillSelectorEditorModule>();
        builder.Register<EditorModule, MoveRangeSelectorEditorModule>();
        builder.Register<EditorModule, MoveSelectorEditorModule>();
        builder.Register<EditorModule, WarriorNameTableEditorModule>();
        builder.Register<EditorModule, BaseWarriorSelectorEditorModule>();
        builder.Register<EditorModule, MaxLinkSelectorEditorModule>();
        builder.Register<EditorModule, MaxLinkPokemonSelectorEditorModule>();
        builder.Register<EditorModule, ScenarioWarriorWorkspaceEditorModule>();
        builder.Register<EditorModule, ScenarioAppearPokemonSelectorEditorModule>();
        builder.Register<EditorModule, ScenarioBuildingSelectorEditorModule>();
        builder.Register<EditorModule, EventSpeakerSelectorEditorModule>();
        builder.Register<EditorModule, ItemSelectorEditorModule>();
        builder.Register<EditorModule, BuildingWorkspaceEditorModule>();
        builder.Register<EditorModule, MsgGridEditorModule>();
        builder.Register<EditorModule, GimmickSelectorEditorModule>();
        builder.Register<EditorModule, EpisodeSelectorEditorModule>();
        builder.Register<EditorModule, KingdomSelectorEditorModule>();
        builder.Register<EditorModule, BattleConfigSelectorEditorModule>();
        builder.Register<EditorModule, GimmickRangeSelectorEditorModule>();
        builder.Register<EditorModule, MapSelectorEditorModule>();
        builder.Register<EditorModule, SpriteEditorModule>();
        builder.Register<EditorModule, BannerEditorModule>();
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
        builder.Register<IMapViewerService, MapViewerService>();

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
        builder.Register<BuildingWorkspaceViewModel>();

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