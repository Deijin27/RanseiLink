using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Services;
using RanseiLink.Core.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("msg", Description = "Get MSG.DAT text.")]
public class MsgCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Block ID.", Name = "block")]
    public int BlockId { get; set; }

    [CommandParameter(1, Description = "Entry ID.", Name = "entry")]
    public int EntryId { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IMsgBlockService>();
        var block = service.Retrieve(BlockId);
        console.Render(block[EntryId]);
        

        return default;
    }
}