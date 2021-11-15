using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.ModelCommands;

[Command("eventspeaker", Description = "Get data on a given event speaker.")]
public class EventSpeakerCommand : BaseCommand
{
    public EventSpeakerCommand(IServiceContainer container) : base(container) { }
    public EventSpeakerCommand() : base() { }

    [CommandParameter(0, Description = "Event Speaker ID.", Name = "id")]
    public EventSpeakerId Id { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        if (!currentModService.TryGetDataService(console, out IDataService dataService))
        {
            return default;
        }

        var model = dataService.EventSpeaker.Retrieve(Id);

        console.Render(model, Id);

        return default;
    }
}
