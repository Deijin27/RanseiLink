using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using RanseiLink.Core.Maps;

namespace RanseiLink.Console.ModelCommands;

[Command("map", Description = "Get data on a given map. If all sections toggles are false, the default is to output all sections.")]
public class MapCommand : BaseCommand
{
    public MapCommand(IServiceContainer container) : base(container) { }
    public MapCommand() : base() { }

    [CommandParameter(0, Description = "Map ID", Name = "map")]
    public int? Map { get; set; }

    [CommandParameter(1, Description = "Map Variant", Name = "variant")]
    public int? Variant { get; set; }

    [CommandOption("header", 'i', Description = "Output the file header")]
    public bool Header { get; set; }

    [CommandOption("pokemon", 'p', Description = "Output the pokemon positions")]
    public bool Pokemon { get; set; }

    [CommandOption("terrain", 't', Description = "Output the terrain")]
    public bool Terrain { get; set; }

    [CommandOption("gimmick", 'g', Description = "Output the gimmicks")]
    public bool Gimmick { get; set; }

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

        string file = Path.Combine(dataService.MapName.MapFolderPath, mapName.ToInternalFileName());

        PSLM.Header header;
        PSLM map;
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            header = new PSLM.Header(br);
            br.BaseStream.Position = 0;
            map = new PSLM(br);
        }

        var all = !(Header || Pokemon || Gimmick || Terrain);

        if (all || Header)
        {
            console.Output.WriteLine(header);
        }

        if (all || Pokemon)
        {
            console.Output.WriteLine("Pokemon Positions:");
            foreach (var position in map.PositionSection.Positions.Reverse().SkipWhile(i => i.X == 0 && i.Y == 0).Reverse())
            {
                console.Output.WriteLine($"- {position}");
            }
        }

        if (all || Terrain)
        {
            console.Output.WriteLine("\nTerrain:\n");
            console.Output.WriteLine(map.TerrainSection);
        }
        
        if (all || Gimmick)
        {
            console.Output.WriteLine("\nGimmicks:\n");
            foreach (var mapGimmickItem in map.GimmickSection.Items)
            {
                console.Output.WriteLine(mapGimmickItem);
            }
        }

        return default;
    }
}
