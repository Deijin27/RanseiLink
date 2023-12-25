#nullable enable
using FluentResults;
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.Windows.Services.Concrete;

public class FallbackSpriteManager(
    IDialogService dialogService,
    IAsyncDialogService asyncDialogService,
    ISettingService settingService, 
    IFallbackDataProvider fallbackDataProvider, 
    IFileDropHandlerFactory fdhFactory
    ) : IFallbackSpriteManager
{
    public void PopulateGraphicsDefaults()
    {
        var vm = new PopulateDefaultSpriteViewModel(asyncDialogService, settingService, fdhFactory);
        if (!dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception? error = null;
        Result? result = null;
        dialogService.ProgressDialog(progress =>
        {
            try
            {
                result = fallbackDataProvider.Populate(vm.File, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Populating Default Sprites",
                message: error.ToString(),
                type: MessageBoxType.Error
                ));
        }
        else if (!result!.IsSuccess)
        {
            dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Failed to Populate Default Sprites",
                message: result.ToString(),
                type: MessageBoxType.Error
                ));
        }
    }

    public void CheckDefaultsPopulated()
    {
        if (EnumUtil.GetValues<ConquestGameCode>().Any(x => fallbackDataProvider.IsDefaultsPopulated(x)))
        {
            return;
        }
        var result = dialogService.ShowMessageBox(new(
            "Welcome To RanseiLink",
            "On of the first steps when using RanseiLink is to 'Populate Graphics Defaults'. " +
            "You provide an unchanged copy of the pokemon conquest rom, and the sprites are extracted from it.\n" +
            "This has a few benefits:\n" +
            "1. You are able to patch with mods that modify sprites\n" +
            "2. You are able to create mods that modify sprites\n" +
            "3. You will see images in various places that improve the experience using this tool\n",
            [
                new("Populate Graphics Now", MessageBoxResult.Yes),
                new("Later", MessageBoxResult.No)
            ],
            DefaultResult: MessageBoxResult.Yes
            ));

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        PopulateGraphicsDefaults();
    }
}
