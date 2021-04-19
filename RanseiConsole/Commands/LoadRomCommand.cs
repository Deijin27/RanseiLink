using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Enums;
using Core.Models;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("loadrom", Description = "Load data from rom into application data.")]
    public class LoadRomCommand : ICommand
    {
        [CommandParameter(0, Description = "Path to rom file.", Name = "path")]
        public string Path { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = new DataService();
            service.LoadRom(Path);

            return default;
        }
    }
}
