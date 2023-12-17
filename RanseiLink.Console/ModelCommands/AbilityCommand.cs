using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("ability", Description = "Get data on a given ability.")]
public class AbilityCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Ability ID.", Name = "id")]
    public AbilityId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IAbilityService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
