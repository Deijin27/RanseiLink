using RanseiLink.Core.Maps;
using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Services.Concrete;

internal class MapIdService : IMapIdService
{
    private readonly ModInfo _mod;
    public MapIdService(ModInfo mod)
    {
        _mod = mod;
    }

    public ICollection<MapId> GetMaps()
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

    public string MapFolderPath => Path.Combine(_mod.FolderPath, Constants.MapFolderPath);
}
