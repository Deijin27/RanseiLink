using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace RanseiLink.Core.Graphics.ExternalFormats
{
    public class OBJ
    {
        private static readonly IFormatProvider _provider = new CultureInfo("en-US");

        public MTL MaterialLib { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();

        public OBJ()
        {
        }

        public OBJ(string file)
        {
            using (StreamReader sr = File.OpenText(file))
            {
                string currentMtl = "";
                Group defaultGroup = new Group() { Name = "default" };
                Group group = defaultGroup;

                string untrimmedLine;
                while ((untrimmedLine = sr.ReadLine()) != null)
                {
                    string line = untrimmedLine.Trim();
                    if (line.Length < 1 || line[0] == '#')
                    {
                        continue;
                    }

                    string[] lineParts = line.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts.Length < 1)
                    {
                        continue;
                    }

                    switch (lineParts[0])
                    {
                        case "mtllib":
                            MaterialLib = new MTL(Path.Combine(Path.GetDirectoryName(file), line.Substring(lineParts[0].Length + 1).Trim()));
                            break;

                        case "g":
                        case "o":
                            if (lineParts.Length >= 2)
                            {
                                group = new Group { Name = lineParts[1] };
                                Groups.Add(group);
                            }
                            break;

                        case "usemtl":
                            if (lineParts.Length >= 2)
                            {
                                currentMtl = lineParts[1];
                            }
                            break;

                        case "v":
                            if (lineParts.Length >= 4)
                            {
                                group.Vertices.Add(new Vector3(float.Parse(lineParts[1], _provider), float.Parse(lineParts[2], _provider), float.Parse(lineParts[3], _provider)));
                            }
                            break;

                        case "vt":
                            if (lineParts.Length >= 2)
                            {
                                group.TextureVertices.Add(new Vector2(float.Parse(lineParts[1], _provider), lineParts.Length < 3 ? 0.0f : float.Parse(lineParts[2], _provider)));
                            }
                            break;

                        case "vn":
                            if (lineParts.Length >= 4)
                            {
                                group.Normals.Add(new Vector3(float.Parse(lineParts[1], _provider), float.Parse(lineParts[2], _provider), float.Parse(lineParts[3], _provider)));
                            }
                            break;

                        case "vc":
                            if (lineParts.Length >= 4)
                            {
                                group.VertexColors.Add(new Rgba32(float.Parse(lineParts[1], _provider), float.Parse(lineParts[2], _provider), float.Parse(lineParts[3], _provider)));
                            }
                            break;

                        case "f":
                            if (lineParts.Length < 4)
                            {
                                break;
                            }
                            var face = new Face
                            {
                                MaterialName = currentMtl
                            };
                            for (int index = 0; index < lineParts.Length - 1; ++index)
                            {
                                string[] indecesParts = lineParts[index + 1].Split('/');
                                face.VertexIndices.Add(int.Parse(indecesParts[0]) - 1);
                                if (indecesParts[1] != "")
                                {
                                    face.TexCoordIndices.Add(int.Parse(indecesParts[1]) - 1);
                                }
                                if (indecesParts.Length > 2 && indecesParts[2] != "")
                                {
                                    face.NormalIndices.Add(int.Parse(indecesParts[2]) - 1);
                                }
                                if (indecesParts.Length > 3)
                                {
                                    face.VertexColorIndices.Add(int.Parse(indecesParts[3]) - 1);
                                }
                            }
                            group.Faces.Add(face);
                            break;
                    }
                }
            
                if (defaultGroup.Vertices.Any())
                {
                    Groups.Insert(0, defaultGroup);
                }
            }

            if (MaterialLib == null)
            {
                var expectedMtlFile = Path.ChangeExtension(file, ".mtl");
                if (File.Exists(expectedMtlFile))
                {
                    MaterialLib = new MTL(expectedMtlFile);
                }
            }
        }

        public void Save(string file)
        {
            using (StreamWriter sw = File.CreateText(file))
            {
                sw.WriteLine("# Wavefront OBJ - File Generated by RanseiLink");
                sw.WriteLine("# Total-Vertex-Count: {0}", Groups.Sum(x => x.Vertices.Count));
                sw.WriteLine("# Total-Normal-Count: {0}", Groups.Sum(x => x.Normals.Count));
                sw.WriteLine("# Total-TextureVertex-Count: {0}", Groups.Sum(x => x.TextureVertices.Count));
                sw.WriteLine("# Total-Face-Count: {0}", Groups.Sum(x => x.Faces.Count));
                sw.WriteLine();
                sw.WriteLine("# Groups:");
                foreach (var group in Groups)
                {
                    sw.WriteLine("# {0} [ Vertices: {1}, Normals: {2}, TexCoords: {3}, Faces: {4} ]", group.Name, group.Vertices.Count, group.Normals.Count, group.TextureVertices.Count, group.Faces.Count);
                }

                if (MaterialLib != null)
                {
                    sw.WriteLine();
                    string mtlFile = Path.ChangeExtension(file, ".mtl");
                    sw.WriteLine("mtllib {0}", Path.GetFileName(mtlFile));
                    MaterialLib.Save(mtlFile);
                }
                

                foreach (var group in Groups)
                {
                    sw.WriteLine();
                    sw.WriteLine("o {0}", group.Name);
                    sw.WriteLine("# Vertices: {0}, Normals: {1}, TexCoords: {2}, Faces: {3}", group.Vertices.Count, group.Normals.Count, group.TextureVertices.Count, group.Faces.Count);

                    foreach (Vector3 vertex in group.Vertices)
                    {
                        sw.WriteLine(string.Format(_provider, "v {0:0.000000} {1:0.000000} {2:0.000000}", vertex.X, vertex.Y, vertex.Z));
                    }

                    foreach (Vector2 texCoord in group.TextureVertices)
                    {
                        sw.WriteLine(string.Format(_provider, "vt {0:0.000000} {1:0.000000}", texCoord.X, texCoord.Y));
                    }

                    foreach (Vector3 normal in group.Normals)
                    {
                        sw.WriteLine(string.Format(_provider, "vn {0:0.000000} {1:0.000000} {2:0.000000}", normal.X, normal.Y, normal.Z));
                    }

                    foreach (var vertexColor in group.VertexColors)
                    {
                        sw.WriteLine(string.Format(_provider, "vc {0:0.000000} {1:0.000000} {2:0.000000}", (float)vertexColor.R / byte.MaxValue, (float)vertexColor.G / byte.MaxValue, (float)vertexColor.B / byte.MaxValue));
                    }

                    string currentMaterial = "";
                    foreach (var face in group.Faces)
                    {
                        if (currentMaterial != face.MaterialName)
                        {
                            currentMaterial = face.MaterialName;
                            sw.WriteLine("usemtl {0}", face.MaterialName);
                        }

                        if (!face.VertexIndices.Any())
                        {
                            continue;
                        }

                        bool hasNormalIndices = face.NormalIndices.Any();
                        bool hasTextCoordIndices = face.TexCoordIndices.Any();
                        bool hasVertexColorIndices = face.VertexColorIndices.Any();

                        sw.Write("f");
                        if (hasNormalIndices && hasTextCoordIndices && hasVertexColorIndices)
                        {
                            for (int i = 0; i < face.VertexIndices.Count; i++)
                            {
                                sw.Write(" {0}/{1}/{2}/{3}", face.VertexIndices[i] + 1, face.TexCoordIndices[i] + 1, face.NormalIndices[i] + 1, face.VertexColorIndices[i] + 1);
                            }
                        }
                        else if (hasNormalIndices && hasTextCoordIndices)
                        {
                            for (int i = 0; i < face.VertexIndices.Count; i++)
                            {
                                sw.Write(" {0}/{1}/{2}", face.VertexIndices[i] + 1, face.TexCoordIndices[i] + 1, face.NormalIndices[i] + 1);
                            }
                        }
                        else if (hasTextCoordIndices)
                        {
                            for (int i = 0; i < face.VertexIndices.Count; i++)
                            {
                                sw.Write(" {0}/{1}", face.VertexIndices[i] + 1, face.TexCoordIndices[i] + 1);
                            }
                        }
                        else if (hasNormalIndices)
                        {
                            for (int i = 0; i < face.VertexIndices.Count; i++)
                            {
                                sw.Write(" {0}//{1}", face.VertexIndices[i] + 1, face.NormalIndices[i] + 1);
                            }
                        }
                        else // only has vertex indices
                        {
                            for (int i = 0; i < face.VertexIndices.Count; i++)
                            {
                                sw.Write(" {0}", face.VertexIndices[i] + 1);
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }
        }

        public class Face
        {
            public string MaterialName { get; set; }
            public List<int> VertexIndices { get; set; } = new List<int>();
            public List<int> NormalIndices { get; set; } = new List<int>();
            public List<int> TexCoordIndices { get; set; } = new List<int>();
            public List<int> VertexColorIndices { get; set; } = new List<int>();
        }

        public class Group
        {
            public string Name { get; set; }

            /// <summary>
            /// v: Defines the position of the vertex in three dimensions (x,y,z). Three floating point numbers. Required.
            /// </summary>
            public List<Vector3> Vertices { get; set; } = new List<Vector3>();

            /// <summary>
            /// vn: Vertex normal, a directional vector associated with a vertex, used to facilitate smooth shading. Three floating point numbers. Optional.
            /// </summary>
            public List<Vector3> Normals { get; set; } = new List<Vector3>();

            /// <summary>
            /// vt: Texture coordinates, also known as UV coordinates. Typically two floating point numbers (u, v). These coordinates are used during rendering to determine how to paint the three-dimensional surface with pixels from a 2D texture map, e.g. an image in a format such as PNG. Optional.
            /// </summary>
            public List<Vector2> TextureVertices { get; set; } = new List<Vector2>();
            public List<Rgba32> VertexColors { get; set; } = new List<Rgba32>();
            public List<Face> Faces { get; set; } = new List<Face>();
        }
    }
}
