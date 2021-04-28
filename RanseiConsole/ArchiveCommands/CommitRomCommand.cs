using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ArchiveCommands
{
    [Command("commitrom", Description = "Commit application data to rom.")]
    public class CommitRomCommand : ICommand
    {
        [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
        public string Path { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = new DataService();
            service.CommitToRom(Path);

            return default;
        }
    }
}
