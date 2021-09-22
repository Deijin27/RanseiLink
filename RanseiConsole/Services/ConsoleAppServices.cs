using Core.Services;

namespace RanseiConsole.Services
{
    public class ConsoleAppServices : IConsoleAppServices
    {
        private static IConsoleAppServices _instance;
        public static IConsoleAppServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConsoleAppServices();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public ConsoleAppServices()
        {
            CoreServices = CoreAppServices.Instance;
            var currentModSlot = CoreServices.Settings.CurrentConsoleModSlot;
            var allModInfo = CoreServices.ModService.GetAllModInfo();
            if (currentModSlot < allModInfo.Count)
            {
                CurrentMod = CoreServices.ModService.GetAllModInfo()[CoreServices.Settings.CurrentConsoleModSlot];
            }
        }
        public ICoreAppServices CoreServices { get; }

        public ModInfo CurrentMod { get; }

    }
}
