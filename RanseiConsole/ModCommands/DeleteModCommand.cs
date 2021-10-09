using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("delete mod", Description = "Delete current mod.")]
    public class DeleteModCommand : BaseCommand
    {
        public DeleteModCommand(IServiceContainer container) : base(container) { }
        public DeleteModCommand() : base() { }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();
            var modService = Container.Resolve<IModService>();

            if (!currentModService.TryGetCurrentMod(console, out ModInfo currentMod))
            {
                return default;
            }

            modService.Delete(currentMod);
            console.Output.WriteLine("Current mod deleted. Info of deleted mod:");
            console.Render(currentMod);
            return default;
        }
    }
}
