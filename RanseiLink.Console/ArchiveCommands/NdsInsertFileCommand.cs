using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.RomFs;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds insert file", Description = "Insert a file into an nds file system, overriding the current data in that location. May be slow for files of different length than existing.")]
public class NdsInsertFileCommand : ICommand
{
    private readonly RomFsFactory _romFsFactory;
    public NdsInsertFileCommand(RomFsFactory romFsFactory)
    {
        _romFsFactory = romFsFactory;
    }

    [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath", Converter = typeof(PathConverter))]
    public string NdsPath { get; set; }

    [CommandParameter(1, Description = "Path of file within nds file system", Name = "DestinationPathInNds", Converter = typeof(PathConverter))]
    public string FilePath { get; set; }

    [CommandParameter(2, Description = "Path of file to insert",  Name = "SourcePath", Converter = typeof(PathConverter))]
    public string SourcePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!File.Exists(SourcePath))
        {
            console.Output.WriteLine($"File not found: {SourcePath}");
        }

        using IRomFs nds = _romFsFactory(NdsPath);
        nds.InsertVariableLengthFile(FilePath, SourcePath);

        console.Output.WriteLine("File insertion complete!");
        return default;
    }
}
