﻿using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("scbg pack", Description = "Pack pngs into an scbg file, creating both the data and the info file")]
public class ScbgPackCommand : BaseCommand
{
    public ScbgPackCommand(IServiceContainer container) : base(container) { }
    public ScbgPackCommand() : base() { }

    [CommandParameter(0, Description = "Path of directory containing png files.", Name = "dirPath")]
    public string DirPath { get; set; }

    [CommandOption("destinationDataFile", 'd', Description = "Optional destination data file to pack to; default is a file in the same location as the folder.")]
    public string DestinationDataFile { get; set; }

    [CommandOption("destinationInfoFile", 'i', Description = "Optional destination info file to pack to; default is a file in the same location as the folder.")]
    public string DestinationInfoFile { get; set; }

    [CommandOption("tiled", 't', Description = "Whether the images are tiled")]
    public bool Tiled { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
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
        
        console.Output.WriteLine("Complete!");

        return default;
    }
}