using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using RanseiLink.Core.Graphics;
using Vector3 = System.Numerics.Vector3;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace RanseiLink.View3D;

public enum ModelRenderOptions
{
    Wireframe = 1,
    Grid = 2
}

public static class ModelRenderer
{
    public static void Draw(NSMDL.Model model, ModelRenderOptions options = 0)
    {
        // No idea what of this actually gets the textures to work
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.PolygonSmooth);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.AlphaTest); // this makes the transparency work
        //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
        GL.AlphaFunc(AlphaFunction.Greater, 0f);
        GL.Disable(EnableCap.CullFace);
        GL.PolygonMode(MaterialFace.FrontAndBack, options.HasFlag(ModelRenderOptions.Wireframe) ? PolygonMode.Line : PolygonMode.Fill);
        // Textures don't show without this:
        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

        var gpu = new GpuState(model.Materials[0]);
        foreach (var renderCommand in model.RenderCommands)
        {
            ProcessCommand(gpu, model, renderCommand);
        }

        GL.Disable(EnableCap.Lighting);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.PolygonSmooth);
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.AlphaTest);
        GL.Disable(EnableCap.Blend);
        GL.Enable(EnableCap.CullFace);
    }

    public static void ProcessCommand(GpuState gpu, NSMDL.Model model, RenderCommand command)
    {
        switch (command.OpCode)
        {
            case RenderOpCode.NOP:
                break;

            case RenderOpCode.END:
                break;

            case RenderOpCode.VISIBILITY:
                break;

            case RenderOpCode.MTX_RESTORE:
                gpu.Restore(command.Parameters[0]);
                break;

            case RenderOpCode.BIND_MATERIAL:
                gpu.CurrentMaterial = model.Materials[command.Parameters[0]];
                GL.BindTexture(TextureTarget.Texture2D, MaterialRegistry.GetRegisteredTexture(gpu.CurrentMaterial));
                GL.MatrixMode(MatrixMode.Texture);
                break;

            case RenderOpCode.DRAW_MESH:
                Vector3 vertex = new();
                var dl = model.Polygons[command.Parameters[0]];
                foreach (var comm in dl.Commands)
                {
                    ProcessPolygonCommand(gpu, comm, ref vertex);
                }
                break;

            case RenderOpCode.MTX_MULT:
                var polymshIdx = command.Parameters[0];
                //var parentId = command.Parameters[1];
                //var unknown = command.Parameters[2];

                int restoreIndex = -1;
                int storeIndex = -1;
                switch (command.Flags)
                {
                    case 0: // 3 params
                        break;
                    case 1: // 4 params
                        storeIndex = command.Parameters[3];
                        break;
                    case 2: // 4 params
                        restoreIndex = command.Parameters[3];
                        break;
                    case 3: // 5 params
                        storeIndex = command.Parameters[3];
                        restoreIndex = command.Parameters[4];
                        break;
                    default:
                        break;
                }

                if (restoreIndex != -1)
                {
                    gpu.Restore(restoreIndex);
                }

                var data = model.Polymeshes[polymshIdx];

                gpu.MultiplyMatrix(data.TRSMatrix);

                if (storeIndex != -1)
                {
                    gpu.Store(storeIndex);
                }

                break;

            case RenderOpCode.UNKNOWN_7:
                break;

            case RenderOpCode.UNKNOWN_8:
                break;

            case RenderOpCode.SKIN:
                break;

            case RenderOpCode.UNKNOWN_10:
                break;

            case RenderOpCode.MTX_SCALE:
                if (command.Flags == 1)
                {
                    // down scale
                    gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.DownScale));
                }
                else
                {
                    // up scale
                    gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.UpScale));
                }
                break;

            case RenderOpCode.UNKNOWN_12:
                break;

            case RenderOpCode.UNKNOWN_13:
                break;

            default:
                break;
        }
    }
    
    private static void DrawVert(GpuState gpu, ref Vector3 vertex)
    {
        var vertToDraw = Vector3.Transform(vertex, gpu.CurrentMatrix);
        GL.Vertex3(vertToDraw.X, vertToDraw.Y, vertToDraw.Z);
    }

    private static void ProcessPolygonCommand(GpuState gpu, PolygonDisplayCommand command, ref Vector3 vertex)
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
                var normal = Vector3.Normalize(Vector3.TransformNormal(CommandParamExtractor.NORMAL(command), gpu.CurrentMatrix));
                GL.Normal3(normal.X, normal.Y, normal.Z);
                break;

            case PolygonDisplayOpCode.TEXCOORD:
                var texCoord = CommandParamExtractor.TEXCOORD(command);
                texCoord.X = texCoord.X / gpu.CurrentMaterial.OrigWidth;
                texCoord.Y = -texCoord.Y / gpu.CurrentMaterial.OrigHeight;
                GL.TexCoord2(texCoord.X, texCoord.Y);
                break;

            case PolygonDisplayOpCode.VTX_16:
                vertex = CommandParamExtractor.VTX_16(command);
                DrawVert(gpu, ref vertex);
                break;

            case PolygonDisplayOpCode.VTX_10:
                vertex = CommandParamExtractor.VTX_10(command);
                DrawVert(gpu, ref vertex);
                break;

            case PolygonDisplayOpCode.VTX_XY:
                Vector3 xy = CommandParamExtractor.VTX_XY(command);
                vertex.X = xy.X;
                vertex.Y = xy.Y;
                DrawVert(gpu, ref vertex);
                break;

            case PolygonDisplayOpCode.VTX_XZ:
                Vector3 xz = CommandParamExtractor.VTX_XZ(command);
                vertex.X = xz.X;
                vertex.Z = xz.Z;
                DrawVert(gpu, ref vertex);
                break;

            case PolygonDisplayOpCode.VTX_YZ:
                Vector3 yz = CommandParamExtractor.VTX_YZ(command);
                vertex.Y = yz.Y;
                vertex.Z = yz.Z;
                DrawVert(gpu, ref vertex);
                break;

            case PolygonDisplayOpCode.VTX_DIFF:
                vertex += CommandParamExtractor.VTX_DIFF(command) / 8;
                DrawVert(gpu, ref vertex);
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
                GL.Begin(ConvertPolyType(CommandParamExtractor.BEGIN_VTXS(command)));
                break;

            case PolygonDisplayOpCode.END_VTXS:
                GL.End();
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

    public static PrimitiveType ConvertPolyType(PolygonType type)
    {
        return type switch
        {
            PolygonType.TRI => PrimitiveType.Triangles,
            PolygonType.QUAD => PrimitiveType.Quads,
            PolygonType.TRI_STRIP => PrimitiveType.TriangleStrip,
            PolygonType.QUAD_STRIP => PrimitiveType.QuadStrip,
            _ => PrimitiveType.Lines,
        };
    }

    
}
