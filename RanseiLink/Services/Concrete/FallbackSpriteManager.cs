using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.ViewModels;
using System;
using System.Linq;

namespace RanseiLink.Services.Concrete;

public class FallbackSpriteManager : IFallbackSpriteManager
{
    private readonly IDialogService _dialogService;
    private readonly ISettingService _settingService;
    private readonly IFallbackDataProvider _fallbackDataProvider;

    public FallbackSpriteManager(IDialogService dialogService, ISettingService settingService, IFallbackDataProvider fallbackDataProvider)
    {
        _dialogService = dialogService;
        _settingService = settingService;
        _fallbackDataProvider = fallbackDataProvider;
    }

    public void PopulateGraphicsDefaults()
    {
        var vm = new PopulateDefaultSpriteViewModel(_dialogService, _settingService);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception error = null;
        PopulateResult result = null;
        _dialogService.ProgressDialog(progress =>
        {
            try
            {
                result = _fallbackDataProvider.Populate(vm.File, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Populating Default Sprites",
                message: error.ToString(),
                type: MessageBoxType.Error
                ));
        }
        else if (!result.Success)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Failed to Populate Default Sprites",
                message: result.FailureReason,
                type: MessageBoxType.Error
                ));
        }
    }

    public void CheckDefaultsPopulated()
    {
        if (EnumUtil.GetValues<ConquestGameCode>().Any(x => _fallbackDataProvider.IsDefaultsPopulated(x)))
        {
            return;
        }
        var result = _dialogService.ShowMessageBox(new MessageBoxSettings(
            "Welcome To RanseiLink",
            "On of the first steps when using RanseiLink is to 'Populate Graphics Defaults'. " +
            "You provide an unchanged copy of the pokemon conquest rom, and the sprites are extracted from it.\n" +
            "This has a few benefits:\n" +
            "1. You are able to patch with mods that modify sprites\n" +
            "2. You are able to create mods that modify sprites\n" +
            "3. You will see images in various places that improve the experience using this tool\n",
            new MessageBoxButton[]
            {
                    new MessageBoxButton("Populate Graphics Now", MessageBoxResult.Yes),
                    new MessageBoxButton("Later", MessageBoxResult.No)
            },
            defaultResult: MessageBoxResult.Yes
            ));

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        PopulateGraphicsDefaults();
    }
}
