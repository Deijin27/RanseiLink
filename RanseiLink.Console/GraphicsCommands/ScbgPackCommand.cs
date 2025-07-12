using RanseiLink.Core.Graphics;

namespace RanseiLink.Console.GraphicsCommands;

[Command("scbg pack", Description = "Pack pngs into an scbg file, creating both the data and the info file")]
public class ScbgPackCommand : ICommand
{
    [CommandParameter(0, Description = "Path of directory containing png files.", Name = "dirPath")]
    public string DirPath { get; set; }

    [CommandOption("destinationDataFile", 'd', Description = "Optional destination data file to pack to; default is a file in the same location as the folder.")]
    public string DestinationDataFile { get; set; }

    [CommandOption("destinationInfoFile", 'i', Description = "Optional destination info file to pack to; default is a file in the same location as the folder.")]
    public string DestinationInfoFile { get; set; }

    [CommandOption("tiled", 't', Description = "Whether the images are tiled")]
    public bool Tiled { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationDataFile == null)
        {
            DestinationDataFile = Path.Combine(Path.GetDirectoryName(DirPath), Path.GetFileNameWithoutExtension(DirPath) + " - PackedData.dat");
        }
        if (DestinationInfoFile == null)
        {
            DestinationInfoFile= Path.Combine(Path.GetDirectoryName(DirPath), Path.GetFileNameWithoutExtension(DirPath) + " - PackedInfo.dat");
        }

        SCBGCollection
            .LoadPngs(DirPath, tiled:Tiled)
            .Save(scbgDataFile:DestinationDataFile, scbgInfoFile:DestinationInfoFile);
        
        console.WriteLine("Complete!");

        return default;
    }
}