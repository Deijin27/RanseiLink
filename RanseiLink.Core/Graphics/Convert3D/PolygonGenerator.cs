using System.Collections.Generic;
using System.Numerics;

namespace RanseiLink.Core.Graphics
{

    public class PolygonGenerator
    {
        private PolygonGenerator() { }

        public static List<PolygonDisplayCommand> Generate(Group group, InverseGpuState gpu)
        {
            var state = new PolygonGenerator();
            state.gpu = gpu;
            foreach (Polygon p in group.Polygons)
            {
                state.ProcessPolygon(p);
            }
            return state.result;
        }

        private readonly List<PolygonDisplayCommand> result = new List<PolygonDisplayCommand>();

        private void ProcessPolygon(Polygon p)
        {
            result.Add(CommandGenerator.BEGIN_VTXS(p.PolyType));

            previousTexCoord = ReverseTex(p.TexCoords[0]);
            previousNormal = ReverseNormal(p.Normals[0]);
            previousVertex = ReverseVertex(p.Vertices[0]);

            result.Add(CommandGenerator.TEXCOORD(previousTexCoord));
            result.Add(CommandGenerator.NORMAL(previousNormal));
            result.Add(CommandGenerator.VTX_16(previousVertex));

            for (int i = 1; i < p.Vertices.Count; i++)
            {
                ProcessDrawCall(p.TexCoords[i], p.Normals[i], p.Vertices[i]);
            }

            result.Add(CommandGenerator.END_VTXS());

            // end the 4-command set
            while (result.Count % 4 != 0)
            {
                result.Add(CommandGenerator.NOP());
            }
        }

        InverseGpuState gpu;
        private Vector2 previousTexCoord;
        private Vector3 previousNormal;
        private Vector3 previousVertex;

        private Vector2 ReverseTex(Vector2 texCoord)
        {
            texCoord.X = gpu.CurrentMaterial.OrigWidth * texCoord.X;
            texCoord.Y = -gpu.CurrentMaterial.OrigHeight * texCoord.Y;

            //    var scaleS = gpu.CurrentMaterial.ScaleS == 0.0 ? 1.0 : gpu.CurrentMaterial.ScaleS;
            //    var scaleT = gpu.CurrentMaterial.ScaleT == 0.0 ? 1.0 : gpu.CurrentMaterial.ScaleT;

            //    texCoord.X = (float)(scaleS * gpu.CurrentMaterial.OrigWidth * texCoord.X) * ((gpu.CurrentMaterial.TexImageParam >> 18 & 1) + 1);
            //    texCoord.Y = (float)(-(scaleT * gpu.CurrentMaterial.OrigHeight) * texCoord.Y) * ((gpu.CurrentMaterial.TexImageParam >> 19 & 1) + 1);

            return texCoord;
        }

        private Vector3 ReverseNormal(Vector3 normal)
        {
            return Vector3.Normalize(Vector3.TransformNormal(normal, gpu.InverseCurrentMatrix));
        }

        private Vector3 ReverseVertex(Vector3 vertex)
        {
            return Vector3.Transform(vertex, gpu.InverseCurrentMatrix);
        }

        private void ProcessDrawCall(Vector2 texCoord, Vector3 normal, Vector3 vertex)
        {
            texCoord = ReverseTex(texCoord);
            normal = ReverseNormal(normal);
            vertex = ReverseVertex(vertex);
            if (texCoord != previousTexCoord)
            {
                result.Add(CommandGenerator.TEXCOORD(texCoord));
                previousTexCoord = texCoord;
            }
            if (normal != previousNormal)
            {
                result.Add(CommandGenerator.NORMAL(normal));
                previousNormal = normal;
            }

            // always process the vertex, even if it's the same as the last, because these are what trigger the draw call
            PolygonDisplayCommand optimalVertexCommand;
            if (vertex.X == previousVertex.X)
            {
                optimalVertexCommand = CommandGenerator.VTX_YZ(vertex);
            }
            else if (vertex.Y == previousVertex.Y)
            {
                optimalVertexCommand = CommandGenerator.VTX_XZ(vertex);
            }
            else if (vertex.Z == previousVertex.Z)
            {
                optimalVertexCommand = CommandGenerator.VTX_XY(vertex);
            }
            else
            {
                optimalVertexCommand = CommandGenerator.VTX_16(vertex);
            }
            result.Add(optimalVertexCommand);
            previousVertex = vertex;
        }
    }
}