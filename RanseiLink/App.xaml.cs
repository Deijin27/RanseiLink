using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Registration;
using RanseiLink.Services;
using System.Windows;

namespace RanseiLink
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string Version = "3 - beta6";

        public App()
        {
            IServiceContainer container = ServiceContainer.Instance;
            container.RegisterCoreServices();
            container.RegisterWpfServices();
        }
    }
}
