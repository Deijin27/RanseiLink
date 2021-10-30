using CliFx.Infrastructure;
using Core.Services;

namespace RanseiConsole.Services.Concrete
{
    public class CurrentModService : ICurrentModService
    {
        private readonly ISettingsService _settingsService;
        private readonly IModService _modService;
        private readonly DataServiceFactory _dataServiceFactory;

        public CurrentModService(ISettingsService settingsService, IModService modService, DataServiceFactory dataServiceFactory)
        {
            _modService = modService;
            _settingsService = settingsService;
            _dataServiceFactory = dataServiceFactory;
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
}
