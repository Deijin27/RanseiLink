using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Models;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands;

[Command("eve unpack")]
public class EveUnpackCommand : ICommand
{

    [CommandParameter(0, Description = "Path of eve file.", Name = "eveFile")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        EVE.Unpack(FilePath);

        console.Output.WriteLine("Complete!");

        return default;
    } 
}
