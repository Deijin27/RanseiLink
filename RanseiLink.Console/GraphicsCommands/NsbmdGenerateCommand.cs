using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.ExternalFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbmd generate", Description = "Generate nsbmd and nsbtx from obj, mtl and texture pngs")]
public class NsbmdGenerateCommand : ICommand
{
    [CommandParameter(0, Name = "sourceFile", Description = "Path of .obj; expects .mtl to be of the same name but with .mtl extension in the same folder. "
                                                         + "The location of the textures is dependent on material. Absolute paths will be respected, and relative " 
                                                         + "paths are expected to be relative to the folder of the .mtl")]
    public string SourceFile { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to generate into; default is a folder in the same location as the source.")]
    public string DestinationFolder { get; set; }

    [CommandOption("transparencyFormat", 't', Description = "Texture format to use for images with transparency")]
    public TexFormat TransparencyFormat { get; set; } = TexFormat.Pltt256;

    [CommandOption("opacityFormat", 'o', Description = "Texture format to use for images without transparency")]
    public TexFormat OpacityFormat { get; set; } = TexFormat.Pltt256;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = SourceFile + " - Generated";
        }
        Directory.CreateDirectory(DestinationFolder);

        string objFile = SourceFile;
        string mtlFile = Path.ChangeExtension(objFile, ".mtl");

        if (objFile == null)
        {
            console.Output.WriteLine($"Could not find .obj file: {objFile}");
            return default;
        }
        if (mtlFile == null)
        {
            console.Output.WriteLine($"Could not find .mtl file: {mtlFile}");
            return default;
        }

        var obj = new OBJ(objFile);
        var mtl = new MTL(mtlFile);

        NSMDL.Model model = new NSMDL.Model() { Name = Path.GetFileNameWithoutExtension(objFile) };
        NSTEX tex = new NSTEX();

        if (!GenerateMaterialsAndNsbtx(mtl, model, tex, console))
        {
            return default;
        }

        var nsbtx = new NSBTX { Texture = tex };
        nsbtx.WriteTo(Path.Combine(DestinationFolder, "0001.nsbtx"));

        GenerateModel(obj, model);

        var nsbmd = new NSBMD
        {
            Model = new NSMDL
            {
                Models = new List<NSMDL.Model> { model }
            }
        };

        nsbmd.WriteTo(Path.Combine(DestinationFolder, "0000.nsbmd"));

        return default;
    }

    private void GenerateModel(OBJ obj, NSMDL.Model model)
    {
        ConvertModels.ExtractInfoFromObj(obj, model);

        var groups = ConvertModels.ObjToIntermediate(obj);

        ModelGenerator.Generate(groups, model);
    }

    private class TexInfo
    {
        public string FilePathAsWrittenInMtl { get; set; }
        public NSTEX.Texture Texture { get; set; }
        public NSTEX.Palette Palette { get; set; }
    }

    private bool GenerateMaterialsAndNsbtx(MTL mtl, NSMDL.Model model, NSTEX nstex, IConsole console)
    {
        var textures = mtl.Materials.Select(x => x.DiffuseTextureMapFile).Distinct().ToArray();
        Array.Sort(textures);
        var texInfos = new List<TexInfo>();
        foreach (var tex in textures)
        {
            string absolutePath;
            if (Path.IsPathRooted(tex))
            {
                absolutePath = tex;
            }
            else
            {
                absolutePath = Path.Combine(Path.GetDirectoryName(SourceFile), tex);
            }
            if (!File.Exists(absolutePath))
            {
                console.Output.WriteLine("Failed to find texture at {0}", absolutePath);
                return false;
            }
            var (texture, palette) = NsbtxGenerateCommand.LoadTextureFromImage(absolutePath, TransparencyFormat, OpacityFormat);
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
                Flag =  NSMDL.Model.Material.MATFLAG.TEXMTX_USE | NSMDL.Model.Material.MATFLAG.WIREFRAME,
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
}
