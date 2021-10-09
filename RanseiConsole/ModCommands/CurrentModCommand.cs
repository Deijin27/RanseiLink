using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using RanseiConsole.Services;
using System.Threading.Tasks;

namespace RanseiConsole.ModCommands
{
    [Command("current mod", Description = "Current Mod.")]
    public class CurrentModCommand : BaseCommand
    {
        public CurrentModCommand(IServiceContainer container) : base(container) { }
        public CurrentModCommand() : base() { }

        public override ValueTask ExecuteAsync(IConsole console)
        {
            var currentModService = Container.Resolve<ICurrentModService>();

            if (!currentModService.TryGetCurrentMod(console, out ModInfo currentMod))
            {
                return default;
            }

            console.Render(currentMod);
            return default;
        }
    }
}
