using OpenTK.Wpf;
using System;
using System.Windows.Input;

namespace RanseiLink.View3D.CameraControls;

public class WpfOrbitCameraAdapter
{
    private readonly OrbitCameraController _cameraController;
    private readonly GLWpfControl _openTkControl;
    private readonly Camera _camera;
    public WpfOrbitCameraAdapter(GLWpfControl openTkControl, OrbitCameraController cameraController, Camera camera)
    {
        _openTkControl = openTkControl;
        _cameraController = cameraController;
        _camera = camera;

        _openTkControl.MouseDown += OpenTkControl_MouseDown;
        _openTkControl.MouseWheel += OpenTkControl_MouseWheel;

        cameraController.Update(camera);
    }

    private double _mouseStartX = 0;
    private double _mouseStartY = 0;
    private double _azimuthalAngleStart;
    private double _polarAngleStart;
    private const double _rotationSpeed = 0.6f;
    private const double _zoomSpeed = 0.1f;

    private void OpenTkControl_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }
        
        var ev = e.GetPosition(_openTkControl);
        _mouseStartX = ev.X;
        _mouseStartY = ev.Y;
        _azimuthalAngleStart = _cameraController.AzimuthalAngle;
        _polarAngleStart = _cameraController.PolarAngle;
        _openTkControl.MouseMove += OpenTkControl_MouseMove;
        _openTkControl.MouseUp += OpenTkControl_MouseUp;
        _openTkControl.CaptureMouse();
    }
    private void OpenTkControl_MouseUp(object sender, MouseEventArgs e)
    {
        _openTkControl.MouseMove -= OpenTkControl_MouseMove;
        _openTkControl.MouseUp -= OpenTkControl_MouseUp;
        _openTkControl.ReleaseMouseCapture();
    }
    private void OpenTkControl_MouseMove(object sender, MouseEventArgs e)
    {
        var point = e.GetPosition(_openTkControl);
        _cameraController.AzimuthalAngle = _azimuthalAngleStart + (point.X - _mouseStartX) * _rotationSpeed;
        _cameraController.PolarAngle = _polarAngleStart + (point.Y - _mouseStartY) * _rotationSpeed;
        UpdateCamera();
    }

    private void OpenTkControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        _cameraController.RadialDistance -= e.Delta * _zoomSpeed * 0.1;
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        _cameraController.Update(_camera);
        CameraUpdated?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CameraUpdated;
}