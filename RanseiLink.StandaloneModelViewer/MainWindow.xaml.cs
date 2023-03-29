using OpenTK.Wpf;
using System;
using System.Windows;
using RanseiLink.Core.Enums;
using RanseiLink.View3D.CameraControls;
using RanseiLink.View3D;
using OpenTK.Graphics.OpenGL;

namespace RanseiLink.StandaloneModelViewer;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly WpfOrbitCameraAdapter _cameraAdapter;
    private readonly OrbitCameraController _cameraController;
    private readonly Camera _camera;
    private readonly SceneRenderer _scene;

    public MainWindow()
    {
        InitializeComponent();

        var settings = new GLWpfControlSettings
        {
            MajorVersion = 2,
            MinorVersion = 1,
        };
        OpenTkControl.Start(settings);
        _camera = new Camera();
        _cameraController = new OrbitCameraController();
        _cameraAdapter = new WpfOrbitCameraAdapter(OpenTkControl, _cameraController, _camera);
        _cameraAdapter.CameraUpdated += (_, __) => UpdateTitle();
        UpdateTitle();
        _scene = new SceneRenderer();
        _scene.LoadScene(BattleConfigId.Aurora);
        
        this.Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        OpenTkControl.Render += OpenTkControl_OnRender;
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
        _camera.AspectRatio = (float)(OpenTkControl.ActualWidth / OpenTkControl.ActualHeight);
        CameraUtil.GLLoadMatrix(_camera);
        _scene.Render();

        // doing GL.Finish() once at the end of render seems to help performance
        // doing it multiple times harms performance
        GL.Finish(); 
    }

    private void UpdateTitle()
    {
        Title = $"Azimuth: {_cameraController.AzimuthalAngle:0.0} | " + 
                $"Polar: {_cameraController.PolarAngle:0.0} | " + 
                $"Radius: {_cameraController.RadialDistance:0.0000} | " + 
                $"Pos: {_camera.Position}";
    }
}
