using System.Numerics;

namespace RanseiLink.Core.Graphics;

public class PolygonRipper
{
    private PolygonRipper(GpuState gpu)
    {
        this.gpu = gpu;
    }

    public static Group Rip(IEnumerable<PolygonDisplayCommand> commands, GpuState gpu)
    {
        var state = new PolygonRipper(gpu);
        state.Process(commands);
        return state.group;
    }

    private void Process(IEnumerable<PolygonDisplayCommand> commands)
    {
        foreach (var command in commands)
        {
            ProcessCommand(command);
        }
    }

    GpuState gpu;
    PolygonType polygonType = PolygonType.NONE;
    readonly Group group = new Group(string.Empty, string.Empty);
    Vector3 vertex;
    Vector3 normal = new Vector3(float.NaN, 0, 0);
    Vector2 texCoord = new Vector2(float.NaN, 0);

    /// <summary>
    /// vector3List1
    /// </summary>
    List<Vector3> vertices = new List<Vector3>();

    /// <summary>
    /// vector3List2
    /// </summary>
    List<Vector3> normals = new List<Vector3>();

    /// <summary>
    /// vector2List
    /// </summary>
    List<Vector2> texCoords = new List<Vector2>();

    void DrawVert()
    {
        var vertToDraw = Vector3.Transform(vertex, gpu.CurrentMatrix);
        vertices.Add(vertToDraw);
        normals.Add(normal);
        texCoords.Add(texCoord);
    }

    private void ProcessCommand(PolygonDisplayCommand command)
    {
        switch (command.OpCode)
        {
            case PolygonDisplayOpCode.NOP:
                break;
            case PolygonDisplayOpCode.MTX_MODE:
                break;
            case PolygonDisplayOpCode.MTX_PUSH:
                break;
            case PolygonDisplayOpCode.MTX_POP:
                break;

            case PolygonDisplayOpCode.MTX_STORE:
                gpu.Store(CommandParamExtractor.MTX_STORE(command));
                break;

            case PolygonDisplayOpCode.MTX_RESTORE:
                gpu.Restore(CommandParamExtractor.MTX_RESTORE(command) & 31);
                break;

            case PolygonDisplayOpCode.MTX_IDENTITY:
                break;
            case PolygonDisplayOpCode.MTX_LOAD_4x4:
                break;
            case PolygonDisplayOpCode.MTX_LOAD_4x3:
                break;
            case PolygonDisplayOpCode.MTX_MULT_4x4:
                break;
            case PolygonDisplayOpCode.MTX_MULT_4x3:
                break;
            case PolygonDisplayOpCode.MTX_MULT_3x3:
                break;

            case PolygonDisplayOpCode.MTX_SCALE:
                Vector3 scale = CommandParamExtractor.MTX_SCALE(command);
                gpu.MultiplyMatrix(Matrix4x4.CreateScale(scale));
                break;

            case PolygonDisplayOpCode.MTX_TRANS:
                Vector3 trans = CommandParamExtractor.MTX_TRANS(command);
                gpu.MultiplyMatrix(Matrix4x4.CreateTranslation(trans));
                break;

            case PolygonDisplayOpCode.COLOR:
                break;

            case PolygonDisplayOpCode.NORMAL:
                normal = Vector3.Normalize(Vector3.TransformNormal(CommandParamExtractor.NORMAL(command), gpu.CurrentMatrix));
                break;

            case PolygonDisplayOpCode.TEXCOORD:
                texCoord = CommandParamExtractor.TEXCOORD(command);
                texCoord.X = texCoord.X / gpu.CurrentMaterial.OrigWidth;
                texCoord.Y = -texCoord.Y / gpu.CurrentMaterial.OrigHeight;
                //var scaleS = gpu.CurrentMaterial.ScaleS == 0.0 ? 1.0 : gpu.CurrentMaterial.ScaleS;
                //var scaleT = gpu.CurrentMaterial.ScaleT == 0.0 ? 1.0 : gpu.CurrentMaterial.ScaleT;
                //texCoord.X = (float)(scaleS / gpu.CurrentMaterial.OrigWidth * texCoord.X) / ((gpu.CurrentMaterial.TexImageParam >> 18 & 1) + 1);
                //texCoord.Y = (float)(-(scaleT / gpu.CurrentMaterial.OrigHeight) * texCoord.Y) / ((gpu.CurrentMaterial.TexImageParam >> 19 & 1) + 1);
                break;

            case PolygonDisplayOpCode.VTX_16:
                vertex = CommandParamExtractor.VTX_16(command);
                DrawVert();
                break;

            case PolygonDisplayOpCode.VTX_10:
                vertex = CommandParamExtractor.VTX_10(command);
                DrawVert();
                break;

            case PolygonDisplayOpCode.VTX_XY:
                Vector3 xy = CommandParamExtractor.VTX_XY(command);
                vertex.X = xy.X;
                vertex.Y = xy.Y;
                DrawVert();
                break;

            case PolygonDisplayOpCode.VTX_XZ:
                Vector3 xz = CommandParamExtractor.VTX_XZ(command);
                vertex.X = xz.X;
                vertex.Z = xz.Z;
                DrawVert();
                break;

            case PolygonDisplayOpCode.VTX_YZ:
                Vector3 yz = CommandParamExtractor.VTX_YZ(command);
                vertex.Y = yz.Y;
                vertex.Z = yz.Z;
                DrawVert();
                break;

            case PolygonDisplayOpCode.VTX_DIFF:
                vertex += CommandParamExtractor.VTX_DIFF(command) / 8;
                DrawVert();
                break;

            case PolygonDisplayOpCode.POLYGON_ATTR:
                break;
            case PolygonDisplayOpCode.TEXIMAGE_PARAM:
                break;
            case PolygonDisplayOpCode.PLTT_BASE:
                break;
            case PolygonDisplayOpCode.DIF_AMB:
                break;
            case PolygonDisplayOpCode.SPE_EMI:
                break;
            case PolygonDisplayOpCode.LIGHT_VECTOR:
                break;
            case PolygonDisplayOpCode.LIGHT_COLOR:
                break;
            case PolygonDisplayOpCode.SHININESS:
                break;

            case PolygonDisplayOpCode.BEGIN_VTXS:
                polygonType = CommandParamExtractor.BEGIN_VTXS(command);
                break;

            case PolygonDisplayOpCode.END_VTXS:
                group.Polygons.Add(new Polygon(polygonType, normals.ToList(), texCoords.ToList(), vertices.ToList()));
                normals.Clear();
                texCoords.Clear();
                vertices.Clear();
                break;

            case PolygonDisplayOpCode.SWAP_BUFFERS:
                break;
            case PolygonDisplayOpCode.VIEWPORT:
                break;
            case PolygonDisplayOpCode.BOX_TEST:
                break;
            case PolygonDisplayOpCode.POS_TEST:
                break;
            case PolygonDisplayOpCode.VEC_TEST:
                break;
            default:
                break;
        }
    }
}