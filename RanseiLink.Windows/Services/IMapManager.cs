using RanseiLink.Core.Maps;

namespace RanseiLink.Windows.Services;

public interface IMapManager
{
    public bool IsOverriden(MapId id);
    public bool ExportPac(MapId id);
    public bool ImportPac(MapId id);
    public bool ExportObj(MapId id);
    public bool ImportObj(MapId id);
    public bool ExportPslm(MapId id);
    public bool ImportPslm(MapId id);
    bool RevertModelToDefault(MapId id);
}
