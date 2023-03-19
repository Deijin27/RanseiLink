using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Numerics;

namespace RanseiLink.Core.Graphics;

[System.Diagnostics.DebuggerDisplay("Group: Name={Name}, Material={MaterialName}")]
public class Group
{
    public Group(string name, string materialName)
    {
        Name = name;
        MaterialName = materialName;
    }
    public string Name { get; set; }

    public string MaterialName { get; set; }

    public List<Polygon> Polygons { get; set; } = new List<Polygon>();
}

public enum PolygonType
{
    TRI = 0,
    QUAD = 1,
    TRI_STRIP = 2,
    QUAD_STRIP = 3,

    NONE = -1
}

public class Polygon
{
    public PolygonType PolyType;
    public List<Vector3> Normals;
    public List<Vector2> TexCoords;
    public List<Vector3> Vertices;
    public List<Rgba32>? Colors;

    public Polygon()
    {
        Normals = new List<Vector3>();
        TexCoords = new List<Vector2>();
        Vertices = new List<Vector3>();
        Colors = null;
    }

    public Polygon(
      PolygonType polyType,
      List<Vector3> normals,
      List<Vector2> texCoords,
      List<Vector3> vertices,
      List<Rgba32>? colors = null)
    {
        this.PolyType = polyType;
        this.Normals = normals;
        this.TexCoords = texCoords;
        this.Vertices = vertices;
        this.Colors = colors;
    }
}

/// <summary>
/// For testing. The normal matrix formatting is garbage
/// </summary>
static class StringExtras
{
    public static string Stringify(this Matrix4x4 mtx)
    {
        return $"Matrix4x4 [[{mtx.M11}, {mtx.M12}, {mtx.M13}, {mtx.M14}], " +
                        $"[{mtx.M21}, {mtx.M22}, {mtx.M23}, {mtx.M24}], " +
                        $"[{mtx.M31}, {mtx.M32}, {mtx.M33}, {mtx.M34}], " +
                        $"[{mtx.M41}, {mtx.M42}, {mtx.M43}, {mtx.M44}]]";
    }
}