using FluentResults;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.ExternalFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Xml.Linq;

namespace RanseiLink.Core.Services;

public static class ModelExtractorGenerator
{
    private const string _patternAnimationFileWithExt = "library_pattern_animations.xml";
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

        var files = Directory.GetFiles(sourceDir);
        var filesByType = new Dictionary<PAC.FileTypeNumber, string>();
        foreach (var file in files)
        {
            var fileTypeNum = PAC.ExtensionToFileTypeNumber(Path.GetExtension(file));
            filesByType[fileTypeNum] = file;
        }

        if (!filesByType.ContainsKey(PAC.FileTypeNumber.NSBTX))
        {
            return Result.Fail($"Could not find .nsbtx file in folder: {sourceDir}");
        }
        if (!filesByType.ContainsKey(PAC.FileTypeNumber.NSBMD))
        {
            return Result.Fail($"Could not find .nsbmd file in folder: {sourceDir}");
        }

        var bmd = new NSBMD(filesByType[PAC.FileTypeNumber.NSBMD]);
        var btx = new NSBTX(filesByType[PAC.FileTypeNumber.NSBTX]);
        NSBTP? btp = null;
        if (filesByType.TryGetValue(PAC.FileTypeNumber.NSBTP, out var btpFile))
        {
            btp = new NSBTP(btpFile);
            ExtractPatternAnim(btp, Path.Combine(destinationFolder, _patternAnimationFileWithExt));
        }

        foreach (var model in bmd.Model.Models)
        {
            var obj = ConvertModels.ModelToObj(model);
            var mtl = ModelExtractorGenerator.ExtractMaterialAndTextures(model, btx, btp?.PatternAnimations, destinationFolder);
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
                case PAC.FileTypeNumber.NSBTP:
                    break;
                case PAC.FileTypeNumber.NSBCA:
                case PAC.FileTypeNumber.NSBMA:
                case PAC.FileTypeNumber.PAT:
                case PAC.FileTypeNumber.CHAR:
                case PAC.FileTypeNumber.NSBTA:
                    File.Copy(type.Value, Path.Combine(destinationFolder, Path.GetFileName(type.Value)), true);
                    break;
            }
        }

        return Result.Ok();
    }

    public static void ExtractPatternAnim(NSBTP nsbtp, string destinationFile)
    {
        var element = nsbtp.PatternAnimations.Serialize();

        var doc = new XDocument(element);

        doc.Save(destinationFile);
    }

    private static void ExtractTexture(NSTEX nstex, string texName, string palName, string destinationFolder, string texFileNameWithExt)
    {
        var tex = nstex.Textures.First(x => x.Name == texName);
        var pal = nstex.Palettes.First(x => x.Name == palName);

        var convPal = new Palette(pal.PaletteData, true);
        ImageUtil.SpriteToPng(Path.Combine(destinationFolder, texFileNameWithExt),
            new SpriteImageInfo(tex.TextureData, convPal, tex.Width, tex.Height,
              IsTiled: false,
              Format: tex.Format));
    }

    public static MTL ExtractMaterialAndTextures(NSMDL.Model model, NSBTX? btx, NSPAT? nspat, string destinationFolder)
    {
        var mtl = new MTL();
        HashSet<string> doneTextures = new HashSet<string>();
        for (int i = 0; i < model.Materials.Count; i++)
        {
            NSMDL.Model.Material material = model.Materials[i];

            var m = new MTL.Material(material.Name)
            {
                AmbientColor = PaletteUtil.To32BitColor(material.Ambient),
                DiffuseColor = PaletteUtil.To32BitColor(material.Diffuse),
                SpecularColor = PaletteUtil.To32BitColor(material.Specular),
                Dissolve = (material.PolyAttr >> 16 & 31) / 31f,
            };
            mtl.Materials.Add(m);

            string texFileNameWithExt = material.Texture + ".png";
            m.AmbientTextureMapFile = texFileNameWithExt;
            m.DiffuseTextureMapFile = texFileNameWithExt;
            m.SpecularTextureMapFile = texFileNameWithExt;
            m.DissolveTextureMapFile = texFileNameWithExt;

            if (btx != null && !doneTextures.Contains(material.Texture))
            {
                doneTextures.Add(material.Texture);
                ExtractTexture(btx.Texture, material.Texture, material.Palette, destinationFolder, texFileNameWithExt);
            }
        }

        // sometimes there is textures in nspat that arent listed in material, export these
        if (btx != null && nspat != null)
        {
            foreach (var kf in nspat.PatternAnimations.SelectMany(x => x.Tracks).SelectMany(x => x.KeyFrames))
            {
                if (!doneTextures.Contains(kf.Texture))
                {
                    doneTextures.Add(kf.Texture);
                    ExtractTexture(btx.Texture, kf.Texture, kf.Palette, destinationFolder, kf.Texture + ".png");
                }
            }
        }
        
        return mtl;
    }

    

    public record TextureLoadResult(NSTEX.Texture Texture, NSTEX.Palette Palette);

    public static Result<TextureLoadResult> LoadTextureFromImage(string file, TexFormatSettings texFormat)
    {
        Image<Rgba32> image;
        try
        {
            image = Image.Load<Rgba32>(file);
        }
        catch (UnknownImageFormatException e)
        {
            return Result.Fail<TextureLoadResult>(e.Message + $" File='{file}'");
        }

        var (imageHasTransparency, imageHasSemiTransparency) = ImageUtil.ImageHasTransparency(image);
        TexFormat format;
        if (imageHasSemiTransparency)
        {
            format = texFormat.SemiTransparencyFormat;
        }
        else if (imageHasTransparency)
        {
            format = texFormat.TransparencyFormat;
        }
        else
        {
            format = texFormat.OpacityFormat;
        }

        var colorZeroTransparent = (imageHasTransparency || imageHasSemiTransparency) && (format == TexFormat.Pltt4 || format == TexFormat.Pltt16 || format == TexFormat.Pltt256);

        var result = LoadTextureFromImageInternal(file, image, format, colorZeroTransparent);

        image.Dispose();

        return result;
    }

    public static Result<TextureLoadResult> LoadTextureFromImageInternal(string file, Image<Rgba32> image, TexFormat format, bool colorZeroTransparent)
    {
        var imgInfo = ImageUtil.SpriteFromImage(image, tiled: false, format: format, color0ToTransparent: colorZeroTransparent);

        string texName = Path.GetFileNameWithoutExtension(file);
        if (string.IsNullOrEmpty(texName))
        {
            return Result.Fail($"Texture name of file '{file}' is empty");
        }
        if (texName.Length > RadixName.Length - 3) // to allow the palette to add the _pl
        {
            return Result.Fail($"Texture name '{texName}' is too long. Max length is {RadixName.Length - 3}");
        }
        var texResult = new NSTEX.Texture
        (
            name: texName,
            textureData: imgInfo.Pixels)
        {
            Color0Transparent = colorZeroTransparent,
            Format = format,
            Width = imgInfo.Width,
            Height = imgInfo.Height,
        };
        var outPal = PaletteUtil.From32bitColors(imgInfo.Palette);
        var palSize = format.PaletteSize();
        if (outPal.Length > palSize)
        {
            return Result.Fail($"Palette of texture {texName} has more than the allowed {palSize} colors");
        }
        Array.Resize(ref outPal, palSize);
        var palResult = new NSTEX.Palette(name: texName + "_pl", paletteData: outPal);

        return Result.Ok(new TextureLoadResult(Texture: texResult, Palette: palResult));
    }

    public static Result ReplaceTextures(string pac, string replacementDirectory, string outputPac)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            // Locate texture
            var ext = PAC.FileTypeNumberToExtension(PAC.FileTypeNumber.NSBTX);
            PAC.Unpack(pac, temp);
            string? nsbtxFile = null;
            foreach (var file in Directory.GetFiles(pac))
            {
                if (Path.GetExtension(file) == ext)
                {
                    nsbtxFile = file;
                    break;
                }
            }
            if (nsbtxFile == null)
            {
                return Result.Fail("Could not locate .nsbtx file inside model .pac");
            }

            var nsbtx = new NSBTX(nsbtxFile);


            // Replace textures, preserving formats
            var newNstex = new NSTEX();
            foreach (var texture in nsbtx.Texture.Textures)
            {
                var texExpectedFile = Path.Combine(replacementDirectory, texture.Name + ".png");
                if (!File.Exists(texExpectedFile))
                {
                    return Result.Fail($"Expected texture file not found '{texExpectedFile}'");
                }

                Image<Rgba32> image;
                try
                {
                    image = Image.Load<Rgba32>(texExpectedFile);
                }
                catch (UnknownImageFormatException e)
                {
                    return Result.Fail(e.Message + $" File='{texExpectedFile}'");
                }

                if (texture.Width != image.Width || texture.Height != image.Height)
                {
                    return Result.Fail($"Image '{texExpectedFile}' is wrong size (expected={texture.Width}x{texture.Height}, actual={image.Width}x{image.Height})");
                }

                var res = LoadTextureFromImageInternal(texExpectedFile, image, texture.Format, texture.Color0Transparent);
                if (res.IsFailed)
                {
                    return Result.Fail($"Failed to load texture from image: {res}");
                }
                newNstex.Textures.Add(res.Value.Texture);
                newNstex.Palettes.Add(res.Value.Palette);
            }

            // Save changes

            nsbtx.Texture = newNstex;

            nsbtx.WriteTo(nsbtxFile);

            PAC.Pack(temp, destinationFile: outputPac, sharedFileCount: 0);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to replace textures of model. Reason: {ex}");
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }

    private record TexInfo(string PngFile, NSTEX.Texture Texture, NSTEX.Palette Palette);

    public static Result GenerateMaterialsAndNsbtx(MTL mtl, NSMDL.Model model, NSTEX nstex, NSPAT? pat, string textureSearchFolder, TexFormatSettings texFormat)
    {
        var textures = new List<string>();
        foreach (var material in mtl.Materials)
        {
            var tex = material.DiffuseTextureMapFile;
            if (tex == null)
            {
                return Result.Fail("Material missing diffuse texture");
            }
            if (!textures.Contains(tex))
            {
                textures.Add(tex);
            }
        }
        // if nspat, get textures from there too.
        if (pat != null)
        {
            var usedTextures = new HashSet<string>(textures.Select(x => Path.GetFileNameWithoutExtension(x)));
            foreach (var t in pat.PatternAnimations.SelectMany(x => x.Tracks).SelectMany(x => x.KeyFrames).Select(x => x.Texture))
            {
                if (!usedTextures.Contains(t))
                {
                    usedTextures.Add(t);
                    var file = Path.Combine(textureSearchFolder, t);
                    if (!File.Exists(file))
                    {
                        return Result.Fail($"Cannot find texture specified in library_pattern_animations: '{file}'");
                    }
                    textures.Add(file);
                }
            }
        }

        textures.Sort();
        var texInfos = new List<TexInfo>();
        foreach (var texPng in textures)
        {
            if (!File.Exists(texPng))
            {
                return Result.Fail(string.Format("Failed to find texture at {0}", texPng));
            }
            var result = ModelExtractorGenerator.LoadTextureFromImage(texPng, texFormat);
            if (result.IsFailed)
            {
                return result.ToResult();
            }
            nstex.Textures.Add(result.Value.Texture);
            nstex.Palettes.Add(result.Value.Palette);
            texInfos.Add(new TexInfo(PngFile: texPng, Texture: result.Value.Texture, Palette: result.Value.Palette));
        }

        foreach (var material in mtl.Materials)
        {
            var texInf = texInfos.Find(x => x.PngFile == material.DiffuseTextureMapFile);
            if (texInf == null)
            {
                return Result.Fail($"Material uses undefined texture '{material.DissolveTextureMapFile}'");
            }
            if (string.IsNullOrEmpty(material.Name))
            {
                return Result.Fail("One of the materials does not have a name");
            }
            if (material.Name.Length > RadixName.Length)
            {
                return Result.Fail($"Material name '{material.Name}' is longer than the maximum 16 characters");
            }
            var nsmtl = new NSMDL.Model.Material(texInf.Texture.Name, texInf.Palette.Name, material.Name)
            {
                ItemTag = 0,
                Diffuse = PaletteUtil.From32BitColor(material.DiffuseColor),
                DiffuseIsDefaultVertexColor = true,
                Ambient = PaletteUtil.From32BitColor(material.AmbientColor),
                Specular = PaletteUtil.From32BitColor(material.SpecularColor),
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
            };

            if (nsmtl.OrigWidth == nsmtl.OrigHeight)
            {
                nsmtl.Flag |= NSMDL.Model.Material.MATFLAG.ORIGWH_SAME;
            }
            model.Materials.Add(nsmtl);
        }

        return Result.Ok();

    }

    public record GenerationSettings
    (
        // input
        string ObjFile,
        // output
        string DestinationFolder,
        IModelGenerator ModelGenerator,

        TexFormatSettings TexFormat,
        //input
        string? PatternAnimation = null,
        string? ModelName = null
        //output
        
    );

    public record TexFormatSettings(
        TexFormat TransparencyFormat = TexFormat.Pltt256,
        TexFormat OpacityFormat = TexFormat.Pltt256,
        TexFormat SemiTransparencyFormat = TexFormat.A3I5
        );

    public static Result GenerateModel(GenerationSettings settings)
    {
        // resolve settings

        if (settings.ObjFile == null || !File.Exists(settings.ObjFile))
        {
            return Result.Fail($"Could not find .obj file: {settings.ObjFile}");
        }

        var modelName = settings.ModelName;
        if (string.IsNullOrEmpty(modelName))
        {
            modelName = Path.GetFileNameWithoutExtension(settings.ObjFile);
        }

        if (settings.ModelGenerator == null)
        {
            return Result.Fail("Could not resolve Model Generator");
        }

        if (string.IsNullOrEmpty(settings.DestinationFolder))
        {
            return Result.Fail("Destination folder is null");
        }

        Directory.CreateDirectory(settings.DestinationFolder);
        var nsbmdFile = Path.Combine(settings.DestinationFolder, modelName + PAC.FileTypeNumberToExtension(PAC.FileTypeNumber.NSBMD));
        var nsbtxFile = Path.Combine(settings.DestinationFolder, modelName + PAC.FileTypeNumberToExtension(PAC.FileTypeNumber.NSBTX));

        var dir = Path.GetDirectoryName(settings.ObjFile);
        if (string.IsNullOrEmpty(dir))
        {
            return Result.Fail($"Could not calculate directory of obj file '{settings.ObjFile}'");
        }
        // load files that may not have handlers, but should be put in the destination
        foreach (var file in Directory.GetFiles(dir))
        {
            var ext = Path.GetExtension(file);
            var num = PAC.ExtensionToFileTypeNumber(ext);
            if (num != PAC.FileTypeNumber.DEFAULT)
            {
                var dest = Path.Combine(settings.DestinationFolder, settings.ModelName + ext);
                File.Copy(file, dest, true);
            }
        }

        // load obj and mtl

        var obj = new OBJ(settings.ObjFile);
        var mtl = obj.MaterialLib;
        if (mtl == null)
        {
            return Result.Fail("Could not resolve MTL file");
        }

        FixMaya(Path.GetFileNameWithoutExtension(settings.ObjFile), obj);

        // load pattern animation if exists
        var patternAnimation = settings.PatternAnimation;
        if (patternAnimation == null)
        {
            patternAnimation = Path.Combine(Path.GetDirectoryName(settings.ObjFile)!, _patternAnimationFileWithExt);
        }
        NSPAT? pat = null;
        if (File.Exists(patternAnimation))
        {
            var doc = XDocument.Load(patternAnimation);
            if (doc.Root == null)
            {
                return Result.Fail("Pattern animaton xml does not contain root element");
            }
            pat = NSPAT.Deserialize(doc.Root);
        }

        // generate model

        NSMDL.Model model = new NSMDL.Model(modelName);
        NSTEX tex = new NSTEX();

        var genRes = ModelExtractorGenerator.GenerateMaterialsAndNsbtx(mtl, model, tex, pat, 
            Path.GetDirectoryName(settings.PatternAnimation)!, 
            settings.TexFormat);
        if (genRes.IsFailed)
        {
            return genRes;
        }

        ConvertModels.ExtractInfoFromObj(obj, model);
        var groups = ConvertModels.ObjToIntermediate(obj);
        settings.ModelGenerator.Generate(groups, model);

        // save model

        var nsbtx = new NSBTX(tex);

        nsbtx.WriteTo(nsbtxFile);

        var nsbmd = new NSBMD(
            new NSMDL
            (
                models: new List<NSMDL.Model> { model }
            )
        );

        nsbmd.WriteTo(nsbmdFile);

        return Result.Ok();
    }

    /// <summary>
    /// Maya adds some prefixes to the material names, which makes them too long.
    /// This is annoying to fix manually, so this here does it automatically.
    /// </summary>
    static void FixMaya(string objName, OBJ obj)
    {
        string prefixName = objName + ":";
        foreach (var face in obj.Groups.SelectMany(x => x.Faces))
        {
            if (face.MaterialName.StartsWith(prefixName))
            {
                face.MaterialName = face.MaterialName.Substring(prefixName.Length);
            }
        }
        if (obj.MaterialLib != null)
        {
            foreach (var material in obj.MaterialLib.Materials)
            {
                if (material.Name.StartsWith(prefixName))
                {
                    material.Name = material.Name.Substring(prefixName.Length);
                }
            }
        }
    }
}
