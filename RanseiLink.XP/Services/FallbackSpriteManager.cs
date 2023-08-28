using FluentResults;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.Core;
using RanseiLink.XP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.XP.Services;

public interface IFallbackSpriteManager
{
    Task CheckDefaultsPopulated();
    Task PopulateGraphicsDefaults();
}

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

    public async Task PopulateGraphicsDefaults()
    {
        var vm = new PopulateDefaultSpriteViewModel(_dialogService, _settingService);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception error = null;
        Result result = null;
        await _dialogService.ProgressDialog(progress =>
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
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Populating Default Sprites",
                message: error.ToString(),
                type: MessageBoxType.Error
                ));
        }
        else if (!result.IsSuccess)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Failed to Populate Default Sprites",
                message: result.ToString(),
                type: MessageBoxType.Error
                ));
        }
    }

    public async Task CheckDefaultsPopulated()
    {
        if (EnumUtil.GetValues<ConquestGameCode>().Any(x => _fallbackDataProvider.IsDefaultsPopulated(x)))
        {
            return;
        }
        var result = await _dialogService.ShowMessageBox(new MessageBoxSettings(
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

        await PopulateGraphicsDefaults().ConfigureAwait(false);
    }
}