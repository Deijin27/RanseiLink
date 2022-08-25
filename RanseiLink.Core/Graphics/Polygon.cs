using RanseiLink.Core.Graphics.ExternalFormats;
using RanseiLink.Core.Util;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace RanseiLink.Core.Graphics
{
    public class Group
    {
        public string Name { get; set; }

        public string MaterialName { get; set; }

        public List<Polygon> Polygons { get; set; } = new List<Polygon>();
    }

    public static class ConvertModels
    {
        public static OBJ ModelToObj(NSMDL.Model model)
        {
            var bmdGroupSet = ModelRipper.Rip(model);

            var obj = new OBJ();

            int vertexId = 0;

            foreach (var bmdGroup in bmdGroupSet)
            {
                var objGroup = new OBJ.Group
                {
                    Name = bmdGroup.Name,
                };

                foreach (var polygon in bmdGroup.Polygons)
                {
                    objGroup.Vertices.AddRange(polygon.Vertex);
                    objGroup.TextureVertices.AddRange(polygon.TexCoords);
                    objGroup.Normals.AddRange(polygon.Normals);

                    switch (polygon.PolyType)
                    {
                        case PolygonType.TRI:
                            for (int j = 0; j < polygon.Vertex.Length; j += 3)
                            {
                                var face = new OBJ.Face { MaterialName = bmdGroup.MaterialName };
                                for (int i = 0; i < 3; i++)
                                {
                                    face.VertexIndices.Add(vertexId);
                                    face.TexCoordIndices.Add(vertexId);
                                    face.NormalIndices.Add(vertexId);
                                    vertexId++;
                                }
                                objGroup.Faces.Add(face);
                            }
                            break;

                        case PolygonType.QUAD:
                            for (int j = 0; j < polygon.Vertex.Length; j += 4)
                            {
                                var face = new OBJ.Face { MaterialName = bmdGroup.MaterialName };
                                for (int i = 0; i < 4; i++)
                                {
                                    face.VertexIndices.Add(vertexId);
                                    face.TexCoordIndices.Add(vertexId);
                                    face.NormalIndices.Add(vertexId);
                                    vertexId++;
                                }
                                objGroup.Faces.Add(face);
                            }
                            break;

                        case PolygonType.TRI_STRIP:
                            throw new NotImplementedException("TriStrip polygon exporter not yet implemented");
                            //for (int j = 0; j + 2 < polygon.Vertex.Length; j += 2)
                            //{
                            //    var face1 = new OBJ.Face { MaterialName = bmdGroup.MaterialName };
                            //    for (int i = 0; i < 3; i++)
                            //    {
                            //        face.VertexIndices.Add(vertexId);
                            //        face.TexCoordIndices.Add(vertexId);
                            //        face.NormalIndices.Add(vertexId);
                            //        vertexId++;
                            //    }
                            //    sw.WriteLine("f" + string.Format(" {0}/{0}/{0}", vertexId + j) + string.Format(" {0}/{0}/{0}", vertexId + j + 1) + string.Format(" {0}/{0}/{0}", vertexId + j + 2));
                            //    if (j + 3 < polygon.Vertex.Length)
                            //    {
                            //        sw.WriteLine("f" + string.Format(" {0}/{0}/{0}", vertexId + j + 1) + string.Format(" {0}/{0}/{0}", vertexId + j + 3) + string.Format(" {0}/{0}/{0}", vertexId + j + 2));
                            //    }
                            //}
                            //vertexId += polygon.Vertex.Length;
                            //break;

                        case PolygonType.QUAD_STRIP:
                            throw new NotImplementedException("QuadStrip polygon exporter not yet implemented");
                            //break;

                        case PolygonType.NONE:
                            break;

                        default:
                            break;
                    }
                }

                obj.Groups.Add(objGroup);
            }

            return obj;
        }
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
        public Vector3[] Normals;
        public Vector2[] TexCoords;
        public Vector3[] Vertex;
        public Rgba32[] Colors;

        public Polygon()
        {
        }

        public Polygon(
          PolygonType PolyType,
          Vector3[] Normals,
          Vector2[] TexCoords,
          Vector3[] Vertex,
          Rgba32[] Colors = null)
        {
            this.PolyType = PolyType;
            this.Normals = Normals;
            this.TexCoords = TexCoords;
            this.Vertex = Vertex;
            this.Colors = Colors;
        }
    }

    public class ModelRipper
    {
        private ModelRipper()
        {

        }

        public static List<Group> Rip(NSMDL.Model model)
        {
            var state = new ModelRipper();
            state.gpu = new GpuState();
            state.model = model;
            state.gpu.CurrentMaterial = model.Materials[0];
            state.Process(model.RenderCommands);
            return state.groups;
        }

        private void Process(IEnumerable<RenderCommand> commands)
        {
            foreach (var command in commands)
            {
                ProcessCommand(command);
            }
        }

        NSMDL.Model model;
        List<Group> groups = new List<Group>();
        GpuState gpu;

        private void ProcessCommand(RenderCommand command)
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
                    break;

                case RenderOpCode.DRAW_MESH:
                    var dl = model.Polygons[command.Parameters[0]];
                    var g = PolygonRipper.Rip(dl.Commands, gpu);
                    g.Name = dl.Name;
                    g.MaterialName = gpu.CurrentMaterial.Name;
                    groups.Add(g);
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


    public class GpuState
    {
        public Matrix4x4[] MatrixStack { get; set; }
        public Matrix4x4 CurrentMatrix { get; set; }
        public NSMDL.Model.Material CurrentMaterial { get; set; }

        public GpuState()
        {
            CurrentMatrix = Matrix4x4.Identity;
            MatrixStack = new Matrix4x4[32];
            for (int i = 0; i < MatrixStack.Length; i++)
            {
                MatrixStack[i] = Matrix4x4.Identity;
            }
        }

        public void Restore(int stackIndex)
        {
            CurrentMatrix = MatrixStack[stackIndex];
        }

        public void Store(int stackIndex)
        {
            MatrixStack[stackIndex] = CurrentMatrix;
        }

        public void MultiplyMatrix(Matrix4x4 mtx)
        {
            CurrentMatrix = mtx * CurrentMatrix;
        }
    }

    public class PolygonRipper
    {
        private PolygonRipper()
        {

        }

        public static Group Rip(IEnumerable<PolygonDisplayCommand> commands, GpuState gpu)
        {
            var state = new PolygonRipper();
            state.gpu = gpu;
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
        PolygonType vMode = PolygonType.NONE;
        readonly Group group = new Group();
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
                case MeshDisplayOpCode.NOP:
                    break;
                case MeshDisplayOpCode.MTX_MODE:
                    break;
                case MeshDisplayOpCode.MTX_PUSH:
                    break;
                case MeshDisplayOpCode.MTX_POP:
                    break;

                case MeshDisplayOpCode.MTX_STORE: // 19
                    gpu.Store(command.Params[0]);
                    break;

                case MeshDisplayOpCode.MTX_RESTORE: // 20
                    gpu.Restore(command.Params[0] & 31);
                    break;

                case MeshDisplayOpCode.MTX_IDENTITY: // 21
                    break;
                case MeshDisplayOpCode.MTX_LOAD_4x4:
                    break;
                case MeshDisplayOpCode.MTX_LOAD_4x3:
                    break;
                case MeshDisplayOpCode.MTX_MULT_4x4:
                    break;
                case MeshDisplayOpCode.MTX_MULT_4x3:
                    break;
                case MeshDisplayOpCode.MTX_MULT_3x3:
                    break;

                case MeshDisplayOpCode.MTX_SCALE:
                    float sx = FixedPoint.Fix_1_19_12(command.Params[0]);
                    float sy = FixedPoint.Fix_1_19_12(command.Params[1]);
                    float sz = FixedPoint.Fix_1_19_12(command.Params[3]);
                    gpu.MultiplyMatrix(Matrix4x4.CreateScale(sx, sy, sz));
                    break;

                case MeshDisplayOpCode.MTX_TRANS:
                    break;

                case MeshDisplayOpCode.COLOR: // 32
                    break;

                case MeshDisplayOpCode.NORMAL: // 33
                    normal.X = FixedPoint.Fix(command.Params[0] & 0x3FF, 1, 0, 9);
                    normal.Y = FixedPoint.Fix((command.Params[0] >> 10) & 0x3FF, 1, 0, 9);
                    normal.Z = FixedPoint.Fix((command.Params[0] >> 20) & 0x3FF, 1, 0, 9);
                    normal = Vector3.TransformNormal(normal, gpu.CurrentMatrix);
                    break;

                case MeshDisplayOpCode.TEXCOORD: // 34
                    var x = FixedPoint.Fix(command.Params[0] & 0xFFFF, 1, 11, 4);
                    var y = FixedPoint.Fix(command.Params[0] >> 16, 1, 11, 4);
                    var scaleS = gpu.CurrentMaterial.ScaleS == 0.0 ? 1.0 : gpu.CurrentMaterial.ScaleS;
                    var scaleT = gpu.CurrentMaterial.ScaleT == 0.0 ? 1.0 : gpu.CurrentMaterial.ScaleT;
                    texCoord.X = (float)(scaleS / gpu.CurrentMaterial.OrigWidth * x) / ((gpu.CurrentMaterial.TexImageParam >> 18 & 1) + 1);
                    texCoord.Y = (float)(-(scaleT / gpu.CurrentMaterial.OrigHeight) * y) / ((gpu.CurrentMaterial.TexImageParam >> 19 & 1) + 1);
                    break;

                case MeshDisplayOpCode.VTX_16:
                    vertex.X = FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF);
                    vertex.Y = FixedPoint.Fix_1_3_12(command.Params[0] >> 16);
                    vertex.Z = FixedPoint.Fix_1_3_12(command.Params[1] & 0xFFFF);
                    DrawVert();
                    break;

                case MeshDisplayOpCode.VTX_10:
                    vertex.X = FixedPoint.Fix(command.Params[0] & 0x3FF, 1, 3, 6);
                    vertex.Y = FixedPoint.Fix(command.Params[0] >> 10 & 0x3FF, 1, 3, 6);
                    vertex.Z = FixedPoint.Fix(command.Params[0] >> 20 & 0x3FF, 1, 3, 6);
                    DrawVert();
                    break;

                case MeshDisplayOpCode.VTX_XY:
                    vertex.X = FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF);
                    vertex.Y = FixedPoint.Fix_1_3_12(command.Params[0] >> 16);
                    DrawVert();
                    break;

                case MeshDisplayOpCode.VTX_XZ:
                    vertex.X = FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF);
                    vertex.Z = FixedPoint.Fix_1_3_12(command.Params[0] >> 16);
                    DrawVert();
                    break;

                case MeshDisplayOpCode.VTX_YZ:
                    vertex.Y = FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF);
                    vertex.Z = FixedPoint.Fix_1_3_12(command.Params[0] >> 16);
                    DrawVert();
                    break;

                case MeshDisplayOpCode.VTX_DIFF:
                    vertex.X += FixedPoint.Fix(command.Params[0] & 0x3FF, 1, 0, 9) / 8;
                    vertex.Y += FixedPoint.Fix(command.Params[0] >> 10 & 0x3FF, 1, 0, 9) / 8;
                    vertex.Y += FixedPoint.Fix(command.Params[0] >> 20 & 0x3FF, 1, 0, 9) / 8;
                    DrawVert();
                    break;

                case MeshDisplayOpCode.POLYGON_ATTR:
                    break;
                case MeshDisplayOpCode.TEXIMAGE_PARAM:
                    break;
                case MeshDisplayOpCode.PLTT_BASE:
                    break;
                case MeshDisplayOpCode.DIF_AMB:
                    break;
                case MeshDisplayOpCode.SPE_EMI:
                    break;
                case MeshDisplayOpCode.LIGHT_VECTOR:
                    break;
                case MeshDisplayOpCode.LIGHT_COLOR:
                    break;
                case MeshDisplayOpCode.SHININESS:
                    break;

                case MeshDisplayOpCode.BEGIN_VTXS:
                    vMode = (PolygonType)command.Params[0];
                    break;

                case MeshDisplayOpCode.END_VTXS:
                    group.Polygons.Add(new Polygon(vMode, normals.ToArray(), texCoords.ToArray(), vertices.ToArray()));
                    normals.Clear();
                    texCoords.Clear();
                    vertices.Clear();
                    break;

                case MeshDisplayOpCode.SWAP_BUFFERS:
                    break;
                case MeshDisplayOpCode.VIEWPORT:
                    break;
                case MeshDisplayOpCode.BOX_TEST:
                    break;
                case MeshDisplayOpCode.POS_TEST:
                    break;
                case MeshDisplayOpCode.VEC_TEST:
                    break;
                default:
                    break;
            }
        }
    }
}