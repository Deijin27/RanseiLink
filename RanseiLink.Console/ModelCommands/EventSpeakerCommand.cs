using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("eventspeaker", Description = "Get data on a given event speaker.")]
public class EventSpeakerCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public EventSpeakerCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Event Speaker ID.", Name = "id")]
    public EventSpeakerId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IEventSpeakerService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
