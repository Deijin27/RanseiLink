using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.PluginModule.Services;
using RanseiLink.XP.Dialogs;
using RanseiLink.XP.DragDrop;
using RanseiLink.XP.Services;
using RanseiLink.XP.ViewModels;

namespace RanseiLink.XP;
public class XPServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.RegisterInstance(CreateDialogLocator());
        builder.Register<IAsyncDialogService, DialogService>(Reuse.Singleton);

        builder.Register<IFolderDropHandler, FolderDropHandler>(Reuse.Singleton);
        builder.Register<IFileDropHandlerFactory, FileDropHandlerFactory>(Reuse.Singleton);

        //builder.Register<IPluginService, PluginService>(Reuse.Singleton);
        builder.Register<IThemeService, ThemeService>(Reuse.Singleton);



        builder.Register<MainWindowViewModel, MainWindowViewModel>(Reuse.Singleton);
        builder.Register<IModSelectionViewModel, ModSelectionViewModel>(Reuse.Singleton);
        builder.Register<IMainEditorViewModel, MainEditorViewModel>(Reuse.Singleton);

        builder.RegisterDelegate(context =>
            new ModListItemViewModelFactory((parent, mod) =>
                new ModListItemViewModel(
                    parent,
                    mod,
                    context.Resolve<IModManager>(),
                    context.Resolve<IModPatchingService>(),
                    context.Resolve<IAsyncDialogService>(),
                    context.Resolve<ISettingService>(),
                    context.Resolve<IPluginLoader>(),
                    context.Resolve<IModServiceGetterFactory>(),
                    context.Resolve<IFileDropHandlerFactory>(),
                    context.Resolve<IFolderDropHandler>()
            )), Reuse.Singleton);
    }

    private static IDialogLocator CreateDialogLocator()
    {
        var locator = new RegistryDialogLocator();

        //locator.Register<ImageListDialog, ImageListViewModel>();
        locator.Register<ModCommitDialog, ModCommitViewModel>();
        locator.Register<ModCreateBasedOnDialog, ModCreateBasedOnViewModel>();
        locator.Register<ModCreationDialog, ModCreationViewModel>();
        locator.Register<ModDeleteDialog, ModDeleteViewModel>();
        locator.Register<ModEditInfoDialog, ModEditInfoViewModel>();
        locator.Register<ModExportDialog, ModExportViewModel>();
        //locator.Register<ModifyMapDimensionsDialog, ModifyMapDimensionsViewModel>();
        locator.Register<ModImportDialog, ModImportViewModel>();
        locator.Register<ModUpgradeDialog, ModUpgradeViewModel>();
        locator.Register<PopulateDefaultSpriteDialog, PopulateDefaultSpriteViewModel>();
        //locator.Register<SimplifyPaletteDialog, SimplifyPaletteViewModel>();

        return locator;
    }
}

public class XPModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
    }
}