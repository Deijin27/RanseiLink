using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RanseiLink.Core.Graphics.ExternalFormats
{
    /// <summary>
    /// Wavefront MTL
    /// </summary>
    public class MTL
    {
        private static readonly IFormatProvider _provider = new CultureInfo("en-US");

        public List<Material> Materials { get; } = new List<Material>();

        public MTL()
        {

        }

        private static Rgba32 ParseColorLine(string[] lineParts, Rgba32 defaultReturn = default)
        {
            if (lineParts.Length >= 4)
            {
                return new Rgba32(float.Parse(lineParts[1], _provider), float.Parse(lineParts[2], _provider), float.Parse(lineParts[3], _provider));
            }
            return defaultReturn;
        }

        private static float ParseFloatLine(string[] lineParts, float defaultReturn = default)
        {
            if (lineParts.Length >= 2)
            {
                return float.Parse(lineParts[1], _provider);
            }
            return defaultReturn;
        }

        private static string ParseFilePathLine(string mtlFile, string line, string[] lineParts)
        {
            string file = line.Substring(lineParts[0].Length + 1).Trim();
            if (Path.IsPathRooted(file))
            {
                return file;
            }
            else
            {
                return Path.Combine(Path.GetDirectoryName(mtlFile)!, file);
            }
        }

        public MTL(string file)
        {
            using (StreamReader sr = File.OpenText(file))
            {
                bool anyMaterials = false;
                Material material = new Material(string.Empty);
                string? untrimmedLine;
                while ((untrimmedLine = sr.ReadLine()) != null)
                {
                    string line = untrimmedLine.Trim();

                    if (line.Length == 0)
                    {
                        // is blank line
                        continue;
                    }

                    if (line[0] == '#')
                    {
                        // is comment
                        continue;
                    }

                    string[] lineParts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineParts.Length < 1)
                    {
                        continue;
                    }

                    switch (lineParts[0])
                    {
                        case "newmtl":
                            if (lineParts.Length >= 2)
                            {
                                if (anyMaterials)
                                {
                                    // add the last loaded material because we are beginning a new one
                                    Materials.Add(material);
                                }
                                anyMaterials = true;
                                material = new Material(lineParts[1]);
                            }
                            break;

                        case "Ka":
                            material.AmbientColor = ParseColorLine(lineParts);
                            break;

                        case "Kd":
                            material.DiffuseColor = ParseColorLine(lineParts);
                            break;

                        case "Ks":
                            material.SpecularColor = ParseColorLine(lineParts);
                            break;

                        case "d":
                            material.Dissolve = ParseFloatLine(lineParts, material.Dissolve);
                            break;

                        case "Tr":
                            material.Dissolve = 1 - ParseFloatLine(lineParts, 1 - material.Dissolve);
                            break;

                        case "map_Ka":
                            material.AmbientTextureMapFile = ParseFilePathLine(file, line, lineParts);
                            break;

                        case "map_Kd":
                            material.DiffuseTextureMapFile = ParseFilePathLine(file, line, lineParts);
                            break;

                        case "map_Ks":
                            material.SpecularTextureMapFile = ParseFilePathLine(file, line, lineParts);
                            break;

                        case "map_d":
                            material.DissolveTextureMapFile = ParseFilePathLine(file, line, lineParts);
                            break;
                    }
                }

                if (material != null)
                {
                    // add the last loaded material because we are at the end of the file
                    Materials.Add(material);
                }
            }
        }

        private string WriteColorLine(string prefix, Rgba32 color)
        {
            return string.Format("{0} {1:0.000000} {2:0.000000} {3:0.000000}", prefix, (float)color.R / byte.MaxValue, (float)color.G / byte.MaxValue, (float)color.B / byte.MaxValue);
        }

        private string WriteFloatLine(string prefix, float num)
        {
            return string.Format(_provider, "{0} {1:0.000000}", prefix, num);
        }

        private string WriteFilePathLine(string prefix, string filePath, string mtlFile)
        {
            // Write relative path if possible
            if (Path.GetDirectoryName(filePath) == Path.GetDirectoryName(mtlFile))
            {
                return $"{prefix} {Path.GetFileName(filePath)}";
            }
            else
            {
                return $"{prefix} {filePath}";
            }
        }

        public void Save(string file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);

            using (StreamWriter sw = File.CreateText(file))
            {
                sw.WriteLine("# Wavefront MTL - File Generated by RanseiLink");
                
                foreach (Material material in Materials)
                {
                    sw.WriteLine();
                    sw.WriteLine($"newmtl {material.Name}");
                    sw.WriteLine(WriteColorLine("Ka", material.AmbientColor));
                    sw.WriteLine(WriteColorLine("Kd", material.DiffuseColor));
                    sw.WriteLine(WriteColorLine("Ks", material.SpecularColor));
                    sw.WriteLine(WriteFloatLine("d", material.Dissolve));
                    if (material.AmbientTextureMapFile != null)
                    {
                        sw.WriteLine(WriteFilePathLine("map_Ka", material.AmbientTextureMapFile, file));
                    }
                    if (material.DiffuseTextureMapFile != null)
                    {
                        sw.WriteLine(WriteFilePathLine("map_Kd", material.DiffuseTextureMapFile, file));
                    }
                    if (material.SpecularTextureMapFile != null)
                    {
                        sw.WriteLine(WriteFilePathLine("map_Ks", material.SpecularTextureMapFile, file));
                    }
                    if (material.DissolveTextureMapFile != null)
                    {
                        sw.WriteLine(WriteFilePathLine("map_d", material.DissolveTextureMapFile, file));
                    }
                }
            }
        }

        public class Material
        {
            public Material(string name)
            {
                Name = name;
            }
            /// <summary>
            /// Name of the material
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Ka: specifies ambient color, to account for light that is scattered about the entire scene [see Wikipedia entry for Phong Reflection Model] using values between 0 and 1 for the RGB components.
            /// </summary>
            public Rgba32 AmbientColor { get; set; }

            /// <summary>
            /// Kd: specifies diffuse color, which typically contributes most of the color to an object [see Wikipedia entry for Diffuse Reflection]. In this example, Kd represents a grey color, which will get modified by a colored texture map specified in the map_Kd statement
            /// </summary>
            public Rgba32 DiffuseColor { get; set; }

            /// <summary>
            /// Ks: specifies specular color, the color seen where the surface is shiny and mirror-like [see Wikipedia entry for Specular Reflection].
            /// </summary>
            public Rgba32 SpecularColor { get; set; }

            /// <summary>
            /// d: specifies a factor for dissolve, how much this material dissolves into the background. A factor of 1.0 is fully opaque. A factor of 0.0 is completely transparent.
            /// </summary>
            public float Dissolve { get; set; } = 1f;

            /// <summary>
            /// map_Ka: specifies a color texture file to be applied to the ambient reflectivity of the material. During rendering, map_Ka values are multiplied by the Ka values to derive the RGB components.
            /// </summary>
            public string? AmbientTextureMapFile { get; set; }

            /// <summary>
            /// map_Kd: specifies a color texture file to be applied to the diffuse reflectivity of the material. During rendering, map_Kd values are multiplied by the Kd values to derive the RGB components.
            /// </summary>
            public string? DiffuseTextureMapFile { get; set; }

            /// <summary>
            /// map_Ks: specifies a color texture file to be applied to the specular reflectivity of the material. During rendering, map_Ks values are multiplied by the Ks values to derive the RGB components.
            /// </summary>
            public string? SpecularTextureMapFile { get; set; }

            /// <summary>
            /// map_d
            /// </summary>
            public string? DissolveTextureMapFile { get; set; }

           
        }
    }
}
