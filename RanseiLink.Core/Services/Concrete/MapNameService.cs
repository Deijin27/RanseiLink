using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Services.Concrete;

internal class MapNameService : IMapNameService
{
    private readonly ModInfo _mod;
    public MapNameService(ModInfo mod)
    {
        _mod = mod;
    }

    public ICollection<MapName> GetMaps()
    {
        var files = Directory.GetFiles(MapFolderPath);
        List<MapName> result = new();
        foreach (var file in files)
        {
            if (MapName.TryParse(Path.GetFileName(file), out var map))
            {
                result.Add(map);
            }
        }

        return result;
    }

    public string MapFolderPath => Path.Combine(_mod.FolderPath, Constants.DataFolderPath, "map");
}
