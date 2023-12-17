using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Console.Services;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using RanseiLink.Core.Maps;
using CliFx;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("map", Description = "Get data on a given map. If all sections toggles are false, the default is to output all sections.")]
public class MapCommand(ICurrentModService currentModService) : ICommand
{
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

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var mapService = services.Get<IMapService>();
        
        var mapNames = mapService.GetMapIds().Where(i => i.Map == Map && i.Variant == Variant).ToList();
        if (!mapNames.Any())
        {
            console.Output.WriteLine("No such map exists :(");
            return default;
        }
        var mapName = mapNames.First();

        string file = Path.Combine(mapService.MapFolderPath, mapName.ToInternalPslmName());

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
