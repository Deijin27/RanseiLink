using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.ExternalFormats;
using RanseiLink.Core.Services;
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

        if (!ModelExtractorGenerator.GenerateMaterialsAndNsbtx(mtl, model, tex, TransparencyFormat, OpacityFormat))
        {
            return default;
        }

        var nsbtx = new NSBTX { Texture = tex };
        nsbtx.WriteTo(Path.Combine(DestinationFolder, $"{model.Name}.nsbtx"));

        ModelExtractorGenerator.GenerateModelData(obj, model);

        var nsbmd = new NSBMD
        {
            Model = new NSMDL
            {
                Models = new List<NSMDL.Model> { model }
            }
        };

        nsbmd.WriteTo(Path.Combine(DestinationFolder, $"{model.Name}.nsbmd"));

        return default;
    }

    




}
