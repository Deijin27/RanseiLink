using RanseiLink.Core.Maps;
using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMapService
{
    PSLM Retrieve(MapId id);
    void Save(MapId id, PSLM model);
    string MapFolderPath { get; }
    public ICollection<MapId> GetMapIds();
}

public class MapService : IMapService
{
    private readonly ModInfo _modInfo;
    public MapService(ModInfo mod) 
    {
        _modInfo = mod;
    }

    public PSLM Retrieve(MapId id)
    {
        string file = Path.Combine(MapFolderPath, id.ToInternalFileName());
        using var br = new BinaryReader(File.OpenRead(file));
        return new PSLM(br);
    }

    public void Save(MapId id, PSLM model)
    {
        string file = Path.Combine(MapFolderPath, id.ToInternalFileName());
        using var bw = new BinaryWriter(File.Create(file));
        model.WriteTo(bw);
    }

    public string MapFolderPath => Path.Combine(_modInfo.FolderPath, Constants.MapFolderPath);

    public ICollection<MapId> GetMapIds()
    {
        var files = Directory.GetFiles(MapFolderPath);
        List<MapId> result = new();
        foreach (var file in files)
        {
            if (MapId.TryParseInternalFileName(Path.GetFileName(file), out var map))
            {
                result.Add(map);
            }
        }

        return result;
    }
}
