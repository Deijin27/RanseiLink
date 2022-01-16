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
    public const string Version = "Danio-beta3";
    public IServiceContainer ServiceContainer { get; }

    public App()
    {
        ServiceContainer = new ServiceContainer();
        ServiceContainer.RegisterCoreServices();
        ServiceContainer.RegisterPluginServices();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Register wpf services here because theme service requires that application resources are already initialized
        ServiceContainer.RegisterWpfServices();
        base.OnStartup(e);
    }
}
