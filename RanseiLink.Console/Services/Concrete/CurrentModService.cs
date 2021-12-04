using CliFx.Infrastructure;
using RanseiLink.Core.Services;

namespace RanseiLink.Console.Services.Concrete;

public class CurrentModService : ICurrentModService
{
    private readonly ISettingsService _settingsService;
    private readonly IModService _modService;
    private readonly DataServiceFactory _dataServiceFactory;

    public CurrentModService(IServiceContainer container)
    {
        _modService = container.Resolve<IModService>();
        _settingsService = container.Resolve<ISettingsService>();
        _dataServiceFactory = container.Resolve<DataServiceFactory>();
    }

    public bool TryGetCurrentMod(IConsole console, out ModInfo mod)
    {
        var currentModSlot = _settingsService.CurrentConsoleModSlot;
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

    public bool TryGetDataService(IConsole console, out IDataService dataService)
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
