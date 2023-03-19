using RanseiLink.Core.Graphics.ExternalFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RanseiLink.Core.Graphics;

public static class ConvertModels
{
    public static void ExtractInfoFromObj(OBJ obj, NSMDL.Model model)
    {
        // calculate some mdlinfo values
        Vector3 vMax = obj.Groups[0].Vertices[0];
        Vector3 vMin = obj.Groups[0].Vertices[0];
        foreach (var vert in obj.Groups.SelectMany(x => x.Vertices))
        {
            model.MdlInfo.NumVertex++;
            if (vert.X < vMin.X) vMin.X = vert.X;
            if (vert.Y < vMin.Y) vMin.Y = vert.Y;
            if (vert.Z < vMin.Z) vMin.Z = vert.Z;

            if (vert.X > vMax.X) vMax.X = vert.X;
            if (vert.Y > vMax.Y) vMax.Y = vert.Y;
            if (vert.Z > vMax.Z) vMax.Z = vert.Z;
        }

        Vector3 whd = vMax - vMin;
        vMin *= model.MdlInfo.DownScale;
        whd *= model.MdlInfo.DownScale;

        model.MdlInfo.BoxX = vMin.X;
        model.MdlInfo.BoxY = vMin.Y;
        model.MdlInfo.BoxZ = vMin.Z;
        model.MdlInfo.BoxW = whd.X;
        model.MdlInfo.BoxH = whd.Y;
        model.MdlInfo.BoxD = whd.Z;

        foreach (var face in obj.Groups.SelectMany(x => x.Faces))
        {
            model.MdlInfo.NumFace++;
            switch (ConvertModels.FaceCountToType(face.VertexIndices.Count))
            {
                case PolygonType.TRI:
                    model.MdlInfo.NumTriangle++;
                    break;
                case PolygonType.QUAD:
                    model.MdlInfo.NumQuad++;
                    break;
            }
        }
    }

    public static List<Group> ObjToIntermediate(OBJ obj)
    {
        List<Group> groups = new List<Group>();

        Vector3[] verts = obj.Groups.SelectMany(x => x.Vertices).ToArray();
        Vector3[] norms = obj.Groups.SelectMany(x => x.Normals).ToArray();
        Vector2[] texts = obj.Groups.SelectMany(x => x.TextureVertices).ToArray();

        foreach (var objGroup in obj.Groups.Where(x => x.Faces.Any()))
        {
            var bmdGroup = new Group(name: objGroup.Name, materialName: objGroup.Faces[0].MaterialName);
            groups.Add(bmdGroup);

            Polygon? poly = null;

            foreach (var face in objGroup.Faces)
            {
                var polyType = FaceCountToType(face.VertexIndices.Count);
                if (poly == null) // handle first time in loop
                {
                    poly = new Polygon { PolyType = polyType };
                    bmdGroup.Polygons.Add(poly);
                }
                else if (polyType != poly.PolyType)
                {
                    poly = new Polygon { PolyType = polyType };
                    bmdGroup.Polygons.Add(poly);
                }

                poly.Vertices.AddRange(face.VertexIndices.Select(index => verts[index]));
                poly.Normals.AddRange(face.NormalIndices.Select(index => norms[index]));
                poly.TexCoords.AddRange(face.TexCoordIndices.Select(index => texts[index]));
            }
        }

        return groups;
    }

    public static PolygonType FaceCountToType(int count)
    {
        switch (count)
        {
            case 3:
                return PolygonType.TRI;
            case 4:
                return PolygonType.QUAD;
            default:
                throw new Exception($"My do not know what is a polygon with {count} sides sowwy");
        }
    }

    public static OBJ IntermediateToObj(List<Group> bmdGroupSet)
    {
        var obj = new OBJ();

        int vertexId = 0;

        foreach (var bmdGroup in bmdGroupSet)
        {
            var objGroup = new OBJ.Group(bmdGroup.Name);

            foreach (var polygon in bmdGroup.Polygons)
            {
                objGroup.Vertices.AddRange(polygon.Vertices);
                objGroup.TextureVertices.AddRange(polygon.TexCoords);
                objGroup.Normals.AddRange(polygon.Normals);

                switch (polygon.PolyType)
                {
                    case PolygonType.TRI:
                        for (int j = 0; j < polygon.Vertices.Count; j += 3)
                        {
                            var face = new OBJ.Face(bmdGroup.MaterialName);
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
                        for (int j = 0; j < polygon.Vertices.Count; j += 4)
                        {
                            var face = new OBJ.Face(bmdGroup.MaterialName);
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

    public static OBJ ModelToObj(NSMDL.Model model)
    {
        var bmdGroupSet = ModelRipper.Rip(model);
        return IntermediateToObj(bmdGroupSet);
    }
}