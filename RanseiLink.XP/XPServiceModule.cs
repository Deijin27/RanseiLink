using DryIoc;
using RanseiLink.Core;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.XP.Dialogs;
using RanseiLink.XP.DragDrop;
using RanseiLink.XP.Services;

namespace RanseiLink.XP;
public class XPServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.RegisterInstance(CreateDialogLocator());
        builder.Register<IAsyncDialogService, DialogService>(Reuse.Singleton);
        builder.Register<IDispatcherService, DispatcherService>(Reuse.Singleton);
        builder.Register<IClipboardService, ClipboardService>(Reuse.Singleton);
        builder.Register<IAppInfoService, AppInfoService>(Reuse.Singleton);

        builder.Register<IFolderDropHandler, FolderDropHandler>(Reuse.Singleton);
        builder.Register<IFileDropHandlerFactory, FileDropHandlerFactory>(Reuse.Singleton);
        builder.Register<IPathToImageConverter, PathToImageConverter>(Reuse.Singleton);

        //builder.Register<IPluginService, PluginService>(Reuse.Singleton);
        builder.Register<IThemeService, ThemeService>(Reuse.Singleton);
    }

    private static IDialogLocator CreateDialogLocator()
    {
        var locator = new RegistryDialogLocator();

        locator.Register<ImageListDialog, ImageListViewModel>();
        locator.Register<ModPatchDialog, ModPatchViewModel>();
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