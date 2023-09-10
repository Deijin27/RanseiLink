using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule;
using RanseiLink.XP.Services;
using RanseiLink.XP.ViewModels;
using RanseiLink.XP.Views;
using System.Reflection;
using RanseiLink.Core.Settings;

namespace RanseiLink.XP;
public partial class App : Application
{
    public static readonly string Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    public static MainWindow MainWindow { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var builder = new Container();
            builder.RegisterModule(new CoreServiceModule());
            builder.RegisterModule(new PluginServiceModule());
            builder.RegisterModule(new XPServiceModule());

            var modServiceGetter = new ModServiceGetterFactory(builder);
            modServiceGetter.AddModule(new CoreModServiceModule());
            builder.RegisterInstance<IModServiceGetterFactory>(modServiceGetter);

            // Update Theme to match setting, this is done in construction
            builder.Resolve<IThemeService>();

            desktop.MainWindow = MainWindow = new MainWindow(builder.Resolve<ISettingService>())
            {
                DataContext = builder.Resolve<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
