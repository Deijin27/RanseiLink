using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule;
using RanseiLink.XP.Views;
using System.Reflection;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore;

namespace RanseiLink.XP;
public partial class App : Application
{
    public static readonly string Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    public static MainWindow MainWindow { get; private set; }
    public static ISettingService SettingService { get; private set; }   
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var builder = new Container();
            builder.RegisterInstance(AppVersion.Parse(Version));
            builder.RegisterModule(new CoreServiceModule());
            builder.RegisterModule(new GuiCoreServiceModule());
            builder.RegisterModule(new PluginServiceModule());
            builder.RegisterModule(new XPServiceModule());

            var modServiceGetter = new ModServiceGetterFactory(builder);
            modServiceGetter.AddModule(new CoreModServiceModule());
            modServiceGetter.AddModule(new GuiCoreModServiceModule());
            modServiceGetter.AddModule(new XPModServiceModule());
            builder.RegisterInstance<IModServiceGetterFactory>(modServiceGetter);

            // Update Theme to match setting, this is done in construction
            builder.Resolve<IThemeService>();

            SettingService = builder.Resolve<ISettingService>();
            var mainWindowVm = builder.Resolve<MainWindowViewModel>();
            mainWindowVm.MainEditorEnabled = false;
            desktop.MainWindow = MainWindow = new MainWindow()
            {
                DataContext = mainWindowVm,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
