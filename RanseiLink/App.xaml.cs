using Autofac;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using RanseiLink.ViewModels;
using System.Windows;

namespace RanseiLink;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public const string Version = "4.0";
    public MainWindowViewModel GetMainWindowViewModel() => ContainerProvider.Container.Resolve<MainWindowViewModel>();

    public App()
    {
        
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Register services here because theme service requires that application resources are already initialized
        var builder = new ContainerBuilder();
        builder.RegisterModule(new CoreServiceModule());
        builder.RegisterModule(new PluginServiceModule());
        builder.RegisterModule(new WpfServiceModule());
        var container = builder.Build();
        ContainerProvider.Container = container;

        base.OnStartup(e);
    }
}
