using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("warriorskill", Description = "Get data on a given warrior skill.")]
public class WarriorSkillCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public WarriorSkillCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "WarriorSkill ID.", Name = "id")]
    public WarriorSkillId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IWarriorSkillService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}