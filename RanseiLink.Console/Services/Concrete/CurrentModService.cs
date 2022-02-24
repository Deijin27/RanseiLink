using CliFx.Infrastructure;
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;

namespace RanseiLink.Console.Services.Concrete;

public class CurrentModService : ICurrentModService
{
    private readonly CurrentConsoleModSlotSetting _currentConsoleModSlotSetting;
    private readonly IModManager _modService;
    private readonly DataServiceFactory _dataServiceFactory;

    public CurrentModService(IServiceContainer container)
    {
        _modService = container.Resolve<IModManager>();
        var settingsService = container.Resolve<ISettingService>();
        _currentConsoleModSlotSetting = settingsService.Get<CurrentConsoleModSlotSetting>();
        _dataServiceFactory = container.Resolve<DataServiceFactory>();
    }

    public bool TryGetCurrentMod(IConsole console, out ModInfo mod)
    {
        var currentModSlot = _currentConsoleModSlotSetting.Value;
        var allModInfo = _modService.GetAllModInfo();
        if (currentModSlot < allModInfo.Count)
        {
            mod = allModInfo[currentModSlot];
            return true;
        }
        mod = default;
        console.Output.WriteLine("No mod selected");
        return false;
    }

    public bool TryGetDataService(IConsole console, out IModServiceContainer dataService)
    {
        if (!TryGetCurrentMod(console, out ModInfo currentMod))
        {
            dataService = default;
            return false;
        }

        dataService = _dataServiceFactory(currentMod);
        return true;
    }
}
