using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Core.Services;
using NLua;
using System.IO;
using System.Threading.Tasks;

namespace RanseiConsole.Commands
{
    [Command("lua", Description = "Run given lua script.")]
    public class LuaCommand : ICommand
    {
        [CommandParameter(0, Description = "Absolute path to entry point script", Name = "path")]
        public string FilePath { get; set; }

        public IDataService Service = null;

        public ValueTask ExecuteAsync(IConsole console)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(FilePath));

            FilePath = Path.GetFileName(FilePath);

            using (var lua = new Lua())
            {
                lua.LoadCLRPackage();
                lua.DoString(@"
                    import('Core', 'Core.Enums')
                    import('Core', 'Core.Models')
                ");

                lua.DoString(@"
		            import = function () end
	            ");
                IDataService service = Service ?? new DataService();

                lua["service"] = service;

                lua.DoFile(FilePath);
            }

            return default;
        }
    }
}
