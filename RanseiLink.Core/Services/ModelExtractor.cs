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
        public static void ExtractModelFromPac(string pac, string destinationFolder)
        {
            string temp = FileUtil.GetTemporaryDirectory();
            PAC.Unpack(pac, temp, true, 4);
            ExtractModelFromFolder(temp, destinationFolder);
            Directory.Delete(temp);
        }

        public static void ExtractModelFromFolder(string sourceDir, string destinationFolder)
        {
            var files = Directory.GetFiles(sourceDir);
            var filesByType = new Dictionary<PAC.FileTypeNumber, string>();
            foreach (var file in files)
            {
                var fileTypeNum = PAC.ExtensionToFileTypeNumber(Path.GetExtension(file));
                filesByType[fileTypeNum] = file;
            }

            if (!filesByType.ContainsKey(PAC.FileTypeNumber.NSBTX))
            {
                throw new System.Exception($"Could not find .nsbtx file in folder: {sourceDir}");
            }
            if (!filesByType.ContainsKey(PAC.FileTypeNumber.NSBMD))
            {
                throw new System.Exception($"Could not find .nsbmd file in folder: {sourceDir}");
            }

            var bmd = new NSBMD(filesByType[PAC.FileTypeNumber.NSBMD]);
            var btx = new NSBTX(filesByType[PAC.FileTypeNumber.NSBTX]);

            foreach (var model in bmd.Model.Models)
            {
                ModelExtractorGenerator.ExtractModel(model, destinationFolder);
                ModelExtractorGenerator.ExtractMaterialAndTextures(model, btx, destinationFolder);
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
        }

        public static void ExtractPatternAnim(string filePath, string destinationFile)
        {
            var nsbtp = new NSBTP(filePath);

            var element = nsbtp.PatternAnimations.Serialize();

            var doc = new XDocument(element);

            doc.Save(destinationFile);
        }

        public static void ExtractModel(NSMDL.Model model, string destinationFolder)
        {
            var obj = ConvertModels.ModelToObj(model);
            obj.Save(Path.Combine(destinationFolder, model.Name + ".obj"));
        }

        public static void ExtractMaterialAndTextures(NSMDL.Model model, NSBTX btx, string destinationFolder)
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
            mtl.Save(Path.Combine(destinationFolder, model.Name + ".mtl"));
        }

        private static bool ImageHasTransparency(Image<Rgba32> image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (image[x, y].A != 255)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static (NSTEX.Texture texture, NSTEX.Palette palette) LoadTextureFromImage(string file, TexFormat transparencyFormat, TexFormat opacityFormat)
        {

            Image<Rgba32> image;
            try
            {
                image = Image.Load<Rgba32>(file);
            }
            catch (UnknownImageFormatException e)
            {
                throw new UnknownImageFormatException(e.Message + $" File='{file}'");
            }

            bool imageHasTransparency = ImageHasTransparency(image);
            bool colorZeroTransparent = imageHasTransparency && (transparencyFormat == TexFormat.Pltt4 || transparencyFormat == TexFormat.Pltt16 || transparencyFormat == TexFormat.Pltt256);
            var format = imageHasTransparency ? transparencyFormat : opacityFormat;

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
                throw new Exception($"Error converting image '{file}'", e);
            }
            image.Dispose();

            var imgInfo = new ImageInfo(pixels, palette.ToArray(), width, height);

            string texName = Path.GetFileNameWithoutExtension(file);
            if (texName.Length > 13) // 14 to allow the palette to add the _pl
            {
                throw new Exception("Texture name is too long. Max length is 13");
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

            return (texResult, palResult);
        }

        private class TexInfo
        {
            public string FilePathAsWrittenInMtl { get; set; }
            public NSTEX.Texture Texture { get; set; }
            public NSTEX.Palette Palette { get; set; }
        }

        public static bool GenerateMaterialsAndNsbtx(MTL mtl, NSMDL.Model model, NSTEX nstex, TexFormat transparencyFormat, TexFormat opacityFormat)
        {
            var textures = mtl.Materials.Select(x => x.DiffuseTextureMapFile).Distinct().ToArray();
            Array.Sort(textures);
            var texInfos = new List<TexInfo>();
            foreach (var tex in textures)
            {
                if (!File.Exists(tex))
                {
                    //console.Output.WriteLine("Failed to find texture at {0}", tex);
                    return false;
                }
                var (texture, palette) = ModelExtractorGenerator.LoadTextureFromImage(tex, transparencyFormat, opacityFormat);
                nstex.Textures.Add(texture);
                nstex.Palettes.Add(palette);
                texInfos.Add(new TexInfo { FilePathAsWrittenInMtl = tex, Texture = texture, Palette = palette });
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

        public static void GenerateModelData(OBJ obj, NSMDL.Model model)
        {
            ConvertModels.ExtractInfoFromObj(obj, model);

            var groups = ConvertModels.ObjToIntermediate(obj);
            ModelGenerator modGen = new MapModelGenerator();
            modGen.Generate(groups, model);
        }
    }
}
