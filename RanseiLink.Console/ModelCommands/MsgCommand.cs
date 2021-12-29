using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Services;
using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands;

[Command("msg", Description = "Get MSG.DAT text.")]
public class MsgCommand : BaseCommand
{
    public MsgCommand(IServiceContainer container) : base(container) { }
    public MsgCommand() : base() { }

    [CommandParameter(0, Description = "Block ID.", Name = "block")]
    public int BlockId { get; set; }

    [CommandParameter(1, Description = "Entry ID.", Name = "entry")]
    public int EntryId { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        if (!currentModService.TryGetDataService(console, out IDataService dataService))
        {
            return default;
        }
        var block = dataService.Msg.Retrieve(BlockId);
        console.Render(block[EntryId]);
        

        return default;
    }
}