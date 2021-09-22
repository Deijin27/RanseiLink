using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("commit mod", Description = "Commit current mod to rom.")]
    public class CommitModCommand : ICommand
    {
        [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
        public string Path { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var service = ConsoleAppServices.Instance;
            var currentMod = service.CurrentMod;
            if (currentMod == null)
            {
                console.Output.WriteLine("No mod selected");
                return default;
            }
            
            service.CoreServices.ModService.Commit(currentMod, Path);
            console.Output.WriteLine("Mod written to rom successfully. The mod that was written was the current mod:");
            console.Render(currentMod);
            return default;
        }
    }
}
