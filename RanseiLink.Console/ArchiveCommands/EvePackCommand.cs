using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Models;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands;

[Command("eve pack")]
public class EvePackCommand : ICommand
{

    [CommandParameter(0, Description = "Path of folder containing necessary files.", Name = "eveFolder")]
    public string EveFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        EVE.Pack(EveFolder);

        console.Output.WriteLine("Complete!");

        return default;
    }
}
