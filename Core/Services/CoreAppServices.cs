using System;
using System.IO;

namespace Core.Services
{
    public class CoreAppServices : ICoreAppServices
    {
        private static ICoreAppServices _instance;
        public static ICoreAppServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CoreAppServices();
                }
                return _instance;
            }
            set => _instance = value;
        }
        public CoreAppServices(string rootFolder)
        {
            RootFolder = rootFolder;
            Directory.CreateDirectory(rootFolder);
            ModService = new ModService(this);
            Settings = new SettingsService(this);
        }

        public CoreAppServices() : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RanseiLink")) { }

        public string RootFolder { get; }
        public IDataService DataService(ModInfo mod) => new DataService(mod);
        public IModService ModService { get; }
        public ISettingsService Settings { get; }

    }
}