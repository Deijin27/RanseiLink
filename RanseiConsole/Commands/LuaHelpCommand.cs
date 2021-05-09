using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("lua help", Description = "View help for using lua scripts.")]
    public class LuaHelpCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine(Resources.LuaHelp);

            return default;
        }
    }
}
