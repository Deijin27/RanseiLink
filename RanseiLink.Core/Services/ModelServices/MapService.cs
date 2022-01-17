using RanseiLink.Core.Maps;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMapService : IModelDataService<MapId, Map>
{
}

public class MapService : IMapService
{
    private readonly IMapIdService _mapIdService;
    public MapService(IMapIdService mapNameService) 
    {
        _mapIdService = mapNameService;
    }

    public Map Retrieve(MapId id)
    {
        string file = Path.Combine(_mapIdService.MapFolderPath, id.ToString());
        using var br = new BinaryReader(File.OpenRead(file));
        return new Map(br);
    }

    public void Save(MapId id, Map model)
    {
        string file = Path.Combine(_mapIdService.MapFolderPath, id.ToString());
        using var bw = new BinaryWriter(File.Create(file));
        model.WriteTo(bw);
    }
}
