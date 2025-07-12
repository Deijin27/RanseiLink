using RanseiLink.Core.Models;

namespace RanseiLink.Console.ArchiveCommands;

[Command("eve unpack")]
public class EveUnpackCommand : ICommand
{

    [CommandParameter(0, Description = "Path of eve file.", Name = "eveFile")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        EVE.Unpack(FilePath);

        console.WriteLine("Complete!");

        return default;
    } 
}
