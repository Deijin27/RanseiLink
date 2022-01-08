using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using RanseiLink.Core.Map;

namespace RanseiLink.Console.ModelCommands;

[Command("map", Description = "Get data on a given map.")]
public class MapCommand : BaseCommand
{
    public MapCommand(IServiceContainer container) : base(container) { }
    public MapCommand() : base() { }

    [CommandParameter(0, Description = "Map ID", Name = "map")]
    public int? Map { get; set; }

    [CommandParameter(1, Description = "Map Variant", Name = "variant")]
    public int? Variant { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var currentModService = Container.Resolve<ICurrentModService>();
        if (!currentModService.TryGetDataService(console, out IDataService dataService))
        {
            return default;
        }
        
        var mapName = dataService.MapName.GetMaps().FirstOrDefault(i => i.Map == Map && i.Variant == Variant);
        if (mapName == null)
        {
            console.Output.WriteLine("No such map exists :(");
            return default;
        }

        string file = Path.Combine(dataService.MapName.MapFolderPath, mapName.ToString());

        using var br = new BinaryReader(File.OpenRead(file));
        var map = new Map(br);

        foreach (var mapGimmickItem in map.GimmickSection.Items)
        {
            console.Output.WriteLine(mapGimmickItem);
        }

        return default;
    }
}
