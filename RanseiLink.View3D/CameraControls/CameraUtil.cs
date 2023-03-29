using OpenTK.Graphics.OpenGL;

namespace RanseiLink.View3D.CameraControls;

public static class CameraUtil
{
    public static void GLLoadMatrix(Camera camera)
    {
        var projectionMatrix = camera.GetProjectionMatrix();
        var viewMatrix = camera.GetViewMatrix();

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref projectionMatrix);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadMatrix(ref viewMatrix);
    }
}
