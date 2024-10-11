#nullable enable
using RanseiLink.Console.Settings;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using System.Diagnostics.CodeAnalysis;

namespace RanseiLink.Console.Services;

public interface ICurrentModService
{
    bool TryGetCurrentMod([NotNullWhen(true)] out ModInfo? currentMod);
    bool TryGetCurrentModServiceGetter([NotNullWhen(true)] out IServiceGetter? currentModKernel);
}

public class CurrentModService(ISettingService settingService, IModManager modManager, IModServiceGetterFactory modKernelFactory) : ICurrentModService
{
    private readonly CurrentConsoleModSlotSetting _currentConsoleModSlotSetting = settingService.Get<CurrentConsoleModSlotSetting>();

    public bool TryGetCurrentMod([NotNullWhen(true)] out ModInfo? currentMod)
    {
        var currentModSlot = _currentConsoleModSlotSetting.Value;
        var allModInfo = modManager.GetAllModInfo();
        if (currentModSlot < allModInfo.Count)
        {
            currentMod = allModInfo[currentModSlot];
            return true;
        }
        currentMod = null;
        return false;
    }

    public bool TryGetCurrentModServiceGetter([NotNullWhen(true)] out IServiceGetter? currentModServiceGetter)
    {
        if (!TryGetCurrentMod(out var currentMod))
        {
            currentModServiceGetter = null;
            return false;
        }
        currentModServiceGetter = modKernelFactory.Create(currentMod);
        return true;
    }
}