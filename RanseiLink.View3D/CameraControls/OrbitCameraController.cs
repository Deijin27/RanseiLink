using OpenTK.Mathematics;

namespace RanseiLink.View3D.CameraControls;

public class OrbitCameraController
{
    private double _polarAngle = -45;
    private double _radialDistance = 10;
    private double _azimuthalAngle = 90;

    public double AzimuthalAngle
    {
        get => _azimuthalAngle;
        set => _azimuthalAngle = value;
    }
    public double PolarAngle
    {
        get => _polarAngle;
        set => _polarAngle = MathHelper.Clamp(value, -179, -1);
    }
    public double RadialDistance
    {
        get => _radialDistance;
        set => _radialDistance = MathHelper.Clamp(value, 1, float.MaxValue / 2);
    }

    public void Update(Camera camera)
    {
        var theta = MathHelper.DegreesToRadians(AzimuthalAngle);
        var phi = MathHelper.DegreesToRadians(-PolarAngle);
        var r = RadialDistance * 100;
        var sinPhi = MathHelper.Sin(phi);
        var cosPhi = MathHelper.Cos(phi);
        var sinTheta = MathHelper.Sin(theta);
        var cosTheta = MathHelper.Cos(theta);
        var x = r * cosTheta * sinPhi;
        var z = r * sinTheta * sinPhi;
        var y = r * cosPhi;

        camera.Up = Vector3.UnitY;
        camera.Target = Vector3.Zero;
        camera.Position = new((float)x, (float)y, (float)z);
    }
}
