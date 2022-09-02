using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.ExternalFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RanseiLink.Core.Services
{
    public static class ModelExtractorGenerator
    {
        public static Result ExtractModelFromPac(string pac, string destinationFolder)
        {
            string temp = FileUtil.GetTemporaryDirectory();
            PAC.Unpack(pac, temp, true, 4);
            var result = ExtractModelFromFolder(temp, destinationFolder);
            Directory.Delete(temp, true);
            return result;
        }

        public static Result ExtractModelFromFolder(string sourceDir, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            Result result = new Result();

            var files = Directory.GetFiles(sourceDir);
            var filesByType = new Dictionary<PAC.FileTypeNumber, string>();
            foreach (var file in files)
            {
                var fileTypeNum = PAC.ExtensionToFileTypeNumber(Path.GetExtension(file));
                filesByType[fileTypeNum] = file;
            }

            if (!filesByType.ContainsKey(PAC.FileTypeNumber.NSBTX))
            {
                result.FailureReason = $"Could not find .nsbtx file in folder: {sourceDir}";
                return result;
            }
            if (!filesByType.ContainsKey(PAC.FileTypeNumber.NSBMD))
            {
                result.FailureReason = $"Could not find .nsbmd file in folder: {sourceDir}";
                return result;
            }

            var bmd = new NSBMD(filesByType[PAC.FileTypeNumber.NSBMD]);
            var btx = new NSBTX(filesByType[PAC.FileTypeNumber.NSBTX]);

            foreach (var model in bmd.Model.Models)
            {
                var obj = ConvertModels.ModelToObj(model);
                var mtl = ModelExtractorGenerator.ExtractMaterialAndTextures(model, btx, destinationFolder);
                obj.MaterialLib = mtl;
                obj.Save(Path.Combine(destinationFolder, model.Name + ".obj"));
            }

            foreach (var type in filesByType)
            {
                switch (type.Key)
                {
                    case PAC.FileTypeNumber.NSBMD:
                    case PAC.FileTypeNumber.NSBTX:
                    case PAC.FileTypeNumber.DEFAULT:
                        break;
                    case PAC.FileTypeNumber.NSBTP:
                        ExtractPatternAnim(type.Value, Path.Combine(destinationFolder, "library_pattern_animations.xml"));
                        break;
                    case PAC.FileTypeNumber.UNKNOWN3:
                    case PAC.FileTypeNumber.NSBMA:
                    case PAC.FileTypeNumber.UNKNOWN5:
                    case PAC.FileTypeNumber.CHAR:
                    case PAC.FileTypeNumber.NSBTA:
                        File.Copy(type.Value, Path.Combine(destinationFolder, Path.GetFileName(type.Value)), true);
                        break;
                }
            }

            result.Success = true;
            return result;
        }

        public static void ExtractPatternAnim(string filePath, string destinationFile)
        {
            var nsbtp = new NSBTP(filePath);

            var element = nsbtp.PatternAnimations.Serialize();

            var doc = new XDocument(element);

            doc.Save(destinationFile);
        }

        public static MTL ExtractMaterialAndTextures(NSMDL.Model model, NSBTX btx, string destinationFolder)
        {
            var mtl = new MTL();
            for (int i = 0; i < model.Materials.Count; i++)
            {
                NSMDL.Model.Material material = model.Materials[i];

                var m = new MTL.Material
                {
                    Name = material.Name,
                    AmbientColor = RawPalette.To32BitColor(material.Ambient),
                    DiffuseColor = RawPalette.To32BitColor(material.Diffuse),
                    SpecularColor = RawPalette.To32BitColor(material.Specular),
                    Dissolve = (material.PolyAttr >> 16 & 31) / 31f,
                };
                mtl.Materials.Add(m);

                string texFileNameWithExt = material.Texture + ".png";
                m.AmbientTextureMapFile = texFileNameWithExt;
                m.DiffuseTextureMapFile = texFileNameWithExt;
                m.SpecularTextureMapFile = texFileNameWithExt;
                m.DissolveTextureMapFile = texFileNameWithExt;

                if (btx != null)
                {
                    var tex = btx.Texture.Textures.First(x => x.Name == material.Texture);
                    var pal = btx.Texture.Palettes.First(x => x.Name == material.Palette);

                    var convPal = RawPalette.To32bitColors(pal.PaletteData);
                    if (tex.Color0Transparent)
                    {
                        convPal[0] = SixLabors.ImageSharp.Color.Transparent;
                    }
                    ImageUtil.SaveAsPng(Path.Combine(destinationFolder, texFileNameWithExt),
                        new ImageInfo(tex.TextureData, convPal, tex.Width, tex.Height),
                        tiled: false,
                        format: tex.Format);
                }

            }
            return mtl;
        }

        private static (bool hasTransparency, bool hasSemiTransparency) ImageHasTransparency(Image<Rgba32> image)
        {
            bool hasTransparency = false;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var a = image[x, y].A;
                    if (a != 255)
                    {
                        if (a != 0)
                        {
                            return (true, true);
                        }
                        else
                        {
                            hasTransparency = true;
                        }
                    }
                }
            }
            return (hasTransparency, false);
        }

        public class TextureLoadResult : Result
        {
            public NSTEX.Texture Texture;
            public NSTEX.Palette Palette;
        }

        public static TextureLoadResult LoadTextureFromImage(string file, TexFormat transparencyFormat, TexFormat opacityFormat, TexFormat semiTransparencyFormat)
        {
            var result = new TextureLoadResult();

            Image<Rgba32> image;
            try
            {
                image = Image.Load<Rgba32>(file);
            }
            catch (UnknownImageFormatException e)
            {
                result.FailureReason = e.Message + $" File='{file}'";
                return result;
            }

            var (imageHasTransparency, imageHasSemiTransparency) = ImageHasTransparency(image);
            TexFormat format;
            if (imageHasSemiTransparency)
            {
                format = semiTransparencyFormat;
            }
            else if (imageHasTransparency)
            {
                format = transparencyFormat;
            }
            else
            {
                format = opacityFormat;
            }

            var colorZeroTransparent = (imageHasTransparency || imageHasSemiTransparency) && (format == TexFormat.Pltt4 || format == TexFormat.Pltt16 || format == TexFormat.Pltt256);

            int width = image.Width;
            int height = image.Height;
            var palette = new List<Rgba32>();
            if (colorZeroTransparent)
            {
                palette.Add(Color.Transparent);
            }

            byte[] pixels;
            try
            {
                pixels = ImageUtil.FromImage(image, palette, PointUtil.GetIndex, format, colorZeroTransparent);
            }
            catch (Exception e)
            {
                result.FailureReason = $"Error converting image '{file}': {e}";
                return result;
            }
            image.Dispose();

            var imgInfo = new ImageInfo(pixels, palette.ToArray(), width, height);

            string texName = Path.GetFileNameWithoutExtension(file);
            if (texName.Length > 13) // 14 to allow the palette to add the _pl
            {
                result.FailureReason = "Texture name is too long. Max length is 13";
                return result;
            }
            var texResult = new NSTEX.Texture
            {
                Name = texName,
                TextureData = pixels,
                Color0Transparent = colorZeroTransparent,
                Format = format,
                Width = width,
                Height = height,
            };
            var outPal = RawPalette.From32bitColors(palette);
            Array.Resize(ref outPal, format.PaletteSize());
            var palResult = new NSTEX.Palette { Name = texName + "_pl", PaletteData = outPal };

            result.Texture = texResult;
            result.Palette = palResult;
            result.Success = true;
            return result;
        }

        private class TexInfo
        {
            public string FilePathAsWrittenInMtl { get; set; }
            public NSTEX.Texture Texture { get; set; }
            public NSTEX.Palette Palette { get; set; }
        }

        public static bool GenerateMaterialsAndNsbtx(MTL mtl, NSMDL.Model model, NSTEX nstex, TexFormat transparencyFormat, TexFormat opacityFormat, TexFormat semiTransparencyFormat, out string failureReason)
        {
            failureReason = null;

            var textures = mtl.Materials.Select(x => x.DiffuseTextureMapFile).Distinct().ToArray();
            Array.Sort(textures);
            var texInfos = new List<TexInfo>();
            foreach (var tex in textures)
            {
                if (!File.Exists(tex))
                {
                    failureReason = string.Format("Failed to find texture at {0}", tex);
                    return false;
                }
                var result = ModelExtractorGenerator.LoadTextureFromImage(tex, transparencyFormat, opacityFormat, semiTransparencyFormat);
                if (!result.Success)
                {
                    failureReason = result.FailureReason;
                    return false;
                }
                nstex.Textures.Add(result.Texture);
                nstex.Palettes.Add(result.Palette);
                texInfos.Add(new TexInfo { FilePathAsWrittenInMtl = tex, Texture = result.Texture, Palette = result.Palette });
            }

            foreach (var material in mtl.Materials)
            {
                var texInf = texInfos.Find(x => x.FilePathAsWrittenInMtl == material.DiffuseTextureMapFile);
                var nsmtl = new NSMDL.Model.Material
                {
                    ItemTag = 0,
                    Diffuse = RawPalette.From32BitColor(material.DiffuseColor),
                    DiffuseIsDefaultVertexColor = true,
                    Ambient = RawPalette.From32BitColor(material.AmbientColor),
                    Specular = RawPalette.From32BitColor(material.SpecularColor),
                    EnableShininessTable = false,
                    Emission = Rgb15.Black,
                    PolyAttr = 0x81 | ((uint)(material.Dissolve * 31) & 0b1_1111) << 16,
                    PolyAttrMask = 0x3F1F_F8FF,
                    TexImageParam = 0x3_0000,
                    TexImageParamMask = 0xFFFF_FFFF,
                    TexPaletteBase = 0,
                    Flag = NSMDL.Model.Material.MATFLAG.TEXMTX_USE | NSMDL.Model.Material.MATFLAG.WIREFRAME,
                    OrigWidth = (ushort)texInf.Texture.Width,
                    OrigHeight = (ushort)texInf.Texture.Height,
                    MagWidth = 1,
                    MagHeight = 1,

                    Texture = texInf.Texture.Name,
                    Palette = texInf.Palette.Name,
                    Name = material.Name
                };

                if (nsmtl.OrigWidth == nsmtl.OrigHeight)
                {
                    nsmtl.Flag |= NSMDL.Model.Material.MATFLAG.ORIGWH_SAME;
                }
                model.Materials.Add(nsmtl);
            }

            return true;

        }

        public class Result
        {
            public bool Success;
            public string FailureReason;
        }

        public class GenerationResult : Result
        {
            public string NsbtxFile;
            public string NsbmdFile;
        }

        public class GenerationSettings
        {
            public string ObjFile;
            public string ModelName;
            public TexFormat TransparencyFormat = TexFormat.Pltt256;
            public TexFormat OpacityFormat = TexFormat.Pltt256;
            public TexFormat SemiTransparencyFormat = TexFormat.A3I5;
            public string DestinationFolder;
            public string NsbtxFile;
            public string NsbmdFile;
            public ModelGenerator ModelGenerator;
        }

        public static GenerationResult GenerateModel(GenerationSettings settings)
        {
            var result = new GenerationResult();

            // resolve settings

            if (settings.ObjFile == null || !File.Exists(settings.ObjFile))
            {
                result.FailureReason = $"Could not find .obj file: {settings.ObjFile}";
                return result;
            }

            if (settings.ModelName == null)
            {
                settings.ModelName = Path.GetFileNameWithoutExtension(settings.ObjFile);
            }

            if (settings.ModelGenerator == null)
            {
                result.FailureReason = "Could not resolve Model Generator";
                return result;
            }

            result.NsbtxFile = settings.NsbtxFile ?? Path.Combine(settings.DestinationFolder, settings.ModelName + NSBTX.FileExtension);
            if (result.NsbtxFile == null)
            {
                result.FailureReason = "Could not resolve NSBTX output file";
                return result;
            }

            result.NsbmdFile = settings.NsbmdFile ?? Path.Combine(settings.DestinationFolder, settings.ModelName + NSBMD.FileExtension);
            if (result.NsbmdFile == null)
            {
                result.FailureReason = "Could not resolve NSBMD output file";
                return result;
            }

            // load obj and mtl

            var obj = new OBJ(settings.ObjFile);
            var mtl = obj.MaterialLib;
            if (mtl == null)
            {
                result.FailureReason = "Could not resolve MTL file";
                return result;
            }

            // generate model

            NSMDL.Model model = new NSMDL.Model() { Name = settings.ModelName };
            NSTEX tex = new NSTEX();

            if (!ModelExtractorGenerator.GenerateMaterialsAndNsbtx(mtl, model, tex, settings.TransparencyFormat, settings.OpacityFormat, settings.SemiTransparencyFormat, out string failureReason))
            {
                result.FailureReason = failureReason;
                return result;
            }

            ConvertModels.ExtractInfoFromObj(obj, model);
            var groups = ConvertModels.ObjToIntermediate(obj);
            settings.ModelGenerator.Generate(groups, model);

            // save model

            var nsbtx = new NSBTX { Texture = tex };

            nsbtx.WriteTo(result.NsbtxFile);

            var nsbmd = new NSBMD
            {
                Model = new NSMDL
                {
                    Models = new List<NSMDL.Model> { model }
                }
            };

            nsbmd.WriteTo(result.NsbmdFile);

            result.Success = true;
            return result;
        }
    }
}
