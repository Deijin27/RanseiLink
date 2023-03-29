using OpenTK.Mathematics;

namespace RanseiLink.View3D.CameraControls;

public class Camera
{
    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;

    public float DepthNear { get; set; } = 1f;
    public float DepthFar { get; set; } = 10000f;

    // The position of the camera
    public Vector3 Position { get; set; } = Vector3.Zero;

    // This is simply the aspect ratio of the viewport, used for the projection matrix. Width / Height
    public float AspectRatio { get; set; } = 1f;

    public Vector3 Up { get; set; } = Vector3.UnitY;

    public Vector3 Target { get; set; } = Vector3.Zero;

    // The field of view (FOV) is the vertical angle of the camera view.
    // This has been discussed more in depth in a previous tutorial,
    // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Target, Up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, DepthNear, DepthFar);
    }
}