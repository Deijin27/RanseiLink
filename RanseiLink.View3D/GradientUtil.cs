using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace RanseiLink.View3D;

public static class GradientUtil
{
    public static void VerticalGradientBackground(Color4 topColor, Color4 middleColor, Color4 bottomColor)
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Lighting);
        GL.Disable(EnableCap.Texture2D);

        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        // 0,0 is middle of screen
        const float top = -1;
        const float middle = 0;
        const float bottom = 1;
        const float left = -1;
        const float right = 1;

        GL.Begin(PrimitiveType.Quads);
        // first quad with gradient top-middle
        GL.Color4(topColor);
        GL.Vertex2(left, top);
        GL.Vertex2(right, top);
        GL.Color4(middleColor);
        GL.Vertex2(right, middle);
        GL.Vertex2(left, middle);
        // second quad with gradient middle-bottom
        GL.Color4(middleColor);
        GL.Vertex2(left, middle);
        GL.Vertex2(right, middle);
        GL.Color4(bottomColor);
        GL.Vertex2(right, bottom);
        GL.Vertex2(left, bottom);
        GL.End();

        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopMatrix();
    }

    public static void DrawGrid(Color4 color, Vector3 position, float cellSize, int cellCount, bool depthTest)
    {
        if (depthTest)
        {
            GL.Enable(EnableCap.DepthTest);
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }

        GL.Disable(EnableCap.Texture2D);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();

        var gridSize = cellCount * cellSize;

        GL.Translate(position.X - gridSize / 2, position.Y, position.Z - gridSize / 2);

        GL.Color4(color);
        GL.Begin(PrimitiveType.Lines);

        for (int i = 0; i < cellCount + 1; i++)
        {
            var current = i * cellSize;

            GL.Vertex3(current, 0, 0);
            GL.Vertex3(current, 0, gridSize);

            GL.Vertex3(0, 0, current);
            GL.Vertex3(gridSize, 0, current);
        }

        GL.End();

        GL.PopMatrix();
    }
}
