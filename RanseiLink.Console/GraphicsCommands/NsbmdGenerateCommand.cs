using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;

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
    [CommandOption("semiTransparencyFormat", 's', Description = "Texture format to use for images with transparency")]
    public TexFormat SemiTransparencyFormat { get; set; } = TexFormat.A3I5;

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = SourceFile + " - Generated";
        }
        Directory.CreateDirectory(DestinationFolder);

        var settings = new ModelExtractorGenerator.GenerationSettings
        (
            ObjFile: SourceFile,
            DestinationFolder: DestinationFolder,
            TransparencyFormat: TransparencyFormat,
            OpacityFormat: OpacityFormat,
            SemiTransparencyFormat: SemiTransparencyFormat,
            ModelGenerator: new MapModelGenerator()
        );

        var result = ModelExtractorGenerator.GenerateModel(settings);

        if (result.IsSuccess == false)
        {
            console.Output.WriteLine($"Generation Failed: {result}");
        }
        else
        {
            console.Output.WriteLine($"Generation Succeeded");
            console.Output.WriteLine();
        }

        return default;
    }
}
