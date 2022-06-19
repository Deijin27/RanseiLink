using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using CliFx;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;
using RanseiLink.Core.Enums;

namespace RanseiLink.Console.Commands;

[Command("test", Description = "Test command")]
public  class TestCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public TestCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IBuildingService>();

        foreach (var id in service.ValidIds())
        {
            var building = service.Retrieve(id);
            console.Output.WriteLine($"\n{(BuildingId)id}: {building.Kingdom}");
            foreach (var referenced in building.Buildings)
            {
                if (referenced == BuildingId.Default)
                {
                    console.Output.WriteLine("- default");
                }
                else
                {
                    console.Output.WriteLine($"- {referenced}: {service.Retrieve((int)referenced).Kingdom}");
                }
            }
        }


        return default;
    }
}
