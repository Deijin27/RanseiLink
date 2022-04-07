using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;

namespace RanseiLink.Console.Services;

public interface ICurrentModService
{
    bool TryGetCurrentMod(out ModInfo currentMod);
    bool TryGetCurrentModServiceGetter(out IServiceGetter currentModKernel);
}

public class CurrentModService : ICurrentModService
{
    private readonly CurrentConsoleModSlotSetting _currentConsoleModSlotSetting;
    private readonly IModManager _modManager;
    private readonly IModServiceGetterFactory _modServiceGetterFactory;
    public CurrentModService(ISettingService settingService, IModManager modManager, IModServiceGetterFactory modKernelFactory)
    {
        _modServiceGetterFactory = modKernelFactory;
        _currentConsoleModSlotSetting = settingService.Get<CurrentConsoleModSlotSetting>();
        _modManager = modManager;
    }

    public bool TryGetCurrentMod(out ModInfo currentMod)
    {
        var currentModSlot = _currentConsoleModSlotSetting.Value;
        var allModInfo = _modManager.GetAllModInfo();
        if (currentModSlot < allModInfo.Count)
        {
            currentMod = allModInfo[currentModSlot];
            return true;
        }
        currentMod = null;
        return false;
    }

    public bool TryGetCurrentModServiceGetter(out IServiceGetter currentModServiceGetter)
    {
        if (!TryGetCurrentMod(out var currentMod))
        {
            currentModServiceGetter = null;
            return false;
        }
        currentModServiceGetter = _modServiceGetterFactory.Create(currentMod);
        return true;
    }
}