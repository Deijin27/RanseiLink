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
        [CommandParameter(0, Description = "Path to entry point script", Name = "path")]
        public string FilePath { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var lua = new Lua();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(FilePath));

            lua.LoadCLRPackage();
            lua.DoString(@"
                import('Core', 'Core.Enums')
                import('Core', 'Core.Models')
            ");

            lua.DoString(@"
		        import = function () end
	        ");

            IDataService service = new DataService();
            lua["service"] = service;

            lua.DoFile(FilePath);

            return default;
        }
    }
}
