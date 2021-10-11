using Core.Services;
using Core.Services.Registration;
using RanseiWpf.Services;
using System.Windows;

namespace RanseiWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string Version = "Minnow-beta1";

        public App()
        {
            IServiceContainer container = ServiceContainer.Instance;
            container.RegisterCoreServices();
            container.RegisterWpfServices();
        }
    }
}
