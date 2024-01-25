using RanseiLink.Core.Graphics;

namespace RanseiLink.Console.GraphicsCommands;

[Command("stl unpack", Description = "Unpack an STL collection into it's constituent images")]
public class StlUnpackCommand : ICommand
{
    [CommandParameter(0, Description = "Path of stl data file.", Name = "stlDataFile")]
    public string FilePath { get; set; }

    [CommandParameter(1, Description = "Path of stl info file.", Name = "stlInfoFile")]
    public string InfoFile { get; set; }

    [CommandParameter(2, Description = "Path of ncer.", Name = "ncerPath")]
    public string NcerPath { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
    public string DestinationFolder { get; set; }

    [CommandOption("tiled", 't', Description = "Whether the images are tiled")]
    public bool Tiled { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + " - Unpacked");
        }
        Directory.CreateDirectory(DestinationFolder);

        NCER ncer = NCER.Load(NcerPath);

        using var br = new BinaryReader(File.OpenRead(FilePath));

        STLCollection
            .Load(FilePath, InfoFile)
            .SaveAsPngs(DestinationFolder, ncer, tiled:Tiled);
        
        console.Output.WriteLine("Complete!");

        return default;
    }
}