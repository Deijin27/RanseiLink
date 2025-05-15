using RanseiLink.Core.Maps;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMapService : IModelService<PSLM>
{
    string MapFolderPath { get; }
    ICollection<MapId> GetMapIds();
    string GetFilePath(MapId mapId);
    void Reload(MapId id);
    void Save(MapId id);
}

public class MapService : IMapService
{
    private readonly Dictionary<MapId, PSLM> _cache = [];
    private readonly ModInfo _modInfo;
    public MapService(ModInfo mod)
    {
        _modInfo = mod;
        Reload();
    }

    public void Reload(MapId id)
    {
        string file = Path.Combine(MapFolderPath, id.ToInternalPslmName());
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            _cache[id] = new PSLM(br);
        }
    }

    public void Save(MapId id)
    {
        Save(id, force: true);
    }

    public void Save(MapId id, bool force)
    {
        var model = _cache[id];
        if (!force && !model.IsDirty)
        {
            return;
        }
        string file = Path.Combine(MapFolderPath, id.ToInternalPslmName());
        var temp = Path.GetTempFileName();
        using (var bw = new BinaryWriter(File.Create(temp)))
        {
            model.WriteTo(bw);
        }
        // safety measure to help protect against corruption
        File.Replace(temp, file, null);
    }

    public string MapFolderPath => Path.Combine(_modInfo.FolderPath, Constants.MapFolderPath);

    public ICollection<MapId> GetMapIds()
    {
        var files = Directory.GetFiles(MapFolderPath);
        var result = new List<MapId>();
        foreach (var file in files)
        {
            if (MapId.TryParseInternalFileName(Path.GetFileName(file), out var map))
            {
                result.Add(map);
            }
        }

        return result;
    }

    public string GetFilePath(MapId mapId)
    {
        return Path.Combine(MapFolderPath, mapId.ToInternalPslmName());
    }

    public PSLM Retrieve(int id)
    {
        return _cache[(MapId)id];
    }

    public IEnumerable<PSLM> Enumerate()
    {
        return _cache.Values;
    }

    public object RetrieveObject(int id)
    {
        return Retrieve(id);
    }

    public bool ValidateId(int id)
    {
        var mid = (MapId)id;
        return GetMapIds().Contains(mid);
    }

    public void Reload()
    {
        _cache.Clear();
        foreach (var id in GetMapIds())
        {
            Reload(id);
        }
    }

    public void Save()
    {
        foreach (var item in _cache)
        {
            Save(item.Key, force: false);
        }
    }

    public string IdToName(int id)
    {
        return ((MapId)id).ToString();
    }

    public IEnumerable<int> ValidIds()
    {
        return GetMapIds().Select(id => (int)id);
    }

    public bool TryGetDefaultId(out int defaultId)
    {
        defaultId = default;
        return false;
    }
}