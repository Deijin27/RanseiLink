using RanseiLink.Core.Maps;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMapService : IModelDataService<MapId, PSLM>
{
}

public class MapService : IMapService
{
    private readonly IMapIdService _mapIdService;
    public MapService(IMapIdService mapNameService) 
    {
        _mapIdService = mapNameService;
    }

    public PSLM Retrieve(MapId id)
    {
        string file = Path.Combine(_mapIdService.MapFolderPath, id.ToExternalFileName());
        using var br = new BinaryReader(File.OpenRead(file));
        return new PSLM(br);
    }

    public void Save(MapId id, PSLM model)
    {
        string file = Path.Combine(_mapIdService.MapFolderPath, id.ToExternalFileName());
        using var bw = new BinaryWriter(File.Create(file));
        model.WriteTo(bw);
    }
}
