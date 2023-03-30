using RanseiLink.Core.Services;
using RanseiLink.Core;
using System.Windows;
using DryIoc;
using RanseiLink.View3D;

namespace RanseiLink.StandaloneModelViewer;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public ISceneRenderer? Scene { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Register services here because theme service requires that application resources are already initialized
        var builder = new Container();
        builder.RegisterModule(new CoreServiceModule());

        var modServiceGetter = new ModServiceGetterFactory(builder);
        modServiceGetter.AddModule(new CoreModServiceModule());
        modServiceGetter.AddModule(new View3DModServiceModule());
        builder.RegisterInstance<IModServiceGetterFactory>(modServiceGetter);

        // Load the scene
        var modManager = builder.Resolve<IModManager>();
        var modServiceGetterInstance = modServiceGetter.Create(modManager.GetAllModInfo()[0]);
        Scene = modServiceGetterInstance.Get<ISceneRenderer>();

        base.OnStartup(e);
    }
}
