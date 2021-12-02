using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Registration;
using RanseiLink.PluginModule.Services.Registration;
using RanseiLink.Services;
using System.Windows;

namespace RanseiLink;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public const string Version = "3.0 - beta6";
    public IServiceContainer ServiceContainer { get; }

    public App()
    {
        ServiceContainer = new ServiceContainer();
        ServiceContainer.RegisterCoreServices();
        ServiceContainer.RegisterPluginServices();
        ServiceContainer.RegisterWpfServices();
    }
}
