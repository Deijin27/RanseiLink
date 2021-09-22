using Core.Services;

namespace RanseiWpf.Services
{
    public class WpfAppServices : IWpfAppServices
    {
        private static IWpfAppServices _instance;
        public static IWpfAppServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WpfAppServices();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public WpfAppServices()
        {
            CoreServices = new CoreAppServices();
            DialogService = new DialogService(CoreServices.Settings);
        }

        public ICoreAppServices CoreServices { get; }
        public IDialogService DialogService { get; }
    }
}
