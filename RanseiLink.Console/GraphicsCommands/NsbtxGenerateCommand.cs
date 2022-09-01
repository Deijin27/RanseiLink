using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbtx generate", Description = "Generate new nsbtx from textures")]
public class NsbtxGenerateCommand : ICommand
{
    [CommandParameter(0, Description = "Path of folder to pack into a nsbtx.", Name = "filePath")]
    public string FolderPath { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional file to pack to; default is a file in the same location as the folder.")]
    public string DestinationFile { get; set; }

    [CommandOption("transparencyFormat", 't', Description = "Texture format to use for images with transparency")]
    public TexFormat TransparencyFormat { get; set; } = TexFormat.Pltt256;

    [CommandOption("opacityFormat", 'o', Description = "Texture format to use for images without transparency")]
    public TexFormat OpacityFormat { get; set; } = TexFormat.Pltt256;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrEmpty(DestinationFile))
        {
            DestinationFile = Path.Combine(Path.GetDirectoryName(FolderPath), Path.GetFileNameWithoutExtension(FolderPath) + ".nsbtx");
        }

        var files = Directory.GetFiles(FolderPath);
        Array.Sort(files);

        var tex0 = new NSTEX();
        foreach ( var file in files)
        {
            var (texture, palette) = ModelExtractorGenerator.LoadTextureFromImage(file, TransparencyFormat, OpacityFormat);
            tex0.Textures.Add(texture);
            tex0.Palettes.Add(palette);
        }
        var btx0 = new NSBTX { Texture = tex0 };
        btx0.WriteTo(DestinationFile);

        return default;
    }

    
}
