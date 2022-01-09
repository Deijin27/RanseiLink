using System.IO;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMapService : IModelDataService<MapName, Map.Map>
{
}

public class MapService : IMapService
{
    private readonly IMapNameService _mapNameService;
    public MapService(IMapNameService mapNameService) 
    {
        _mapNameService = mapNameService;
    }

    public Map.Map Retrieve(MapName id)
    {
        string file = Path.Combine(_mapNameService.MapFolderPath, id.ToString());
        using var br = new BinaryReader(File.OpenRead(file));
        return new Map.Map(br);
    }

    public void Save(MapName id, Map.Map model)
    {
        string file = Path.Combine(_mapNameService.MapFolderPath, id.ToString());
        using var bw = new BinaryWriter(File.Create(file));
        model.WriteTo(bw);
    }
}
