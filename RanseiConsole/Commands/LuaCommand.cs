using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using NLua;
using RanseiConsole.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("lua", Description = "Run given lua script.")]
    public class LuaCommand : ICommand
    {
        [CommandParameter(0, Description = "Absolute path to entry point script", Name = "path")]
        public string FilePath { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(FilePath));

            FilePath = Path.GetFileName(FilePath);

            var services = ConsoleAppServices.Instance;

            using (var lua = new Lua())
            {
                lua.LoadCLRPackage();
                lua.DoString(@"
                    import('Core', 'Core.Enums')
                    import('Core', 'Core.Models')
                    import = function () end
                ");

                lua["service"] = services.CoreServices.DataService(services.CurrentMod);

                lua.DoFile(FilePath);
            }

            console.Output.WriteLine("Script executed successfully.");
            return default;
        }
    }
}
