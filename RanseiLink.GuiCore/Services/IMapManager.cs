#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;

namespace RanseiLink.GuiCore.Services;

public interface IMapManager
{
    bool IsOverriden(MapId id);
    Task<bool> ExportPac(MapId id);
    Task<bool> ImportPac(MapId id);
    Task<bool> ExportObj(MapId id);
    Task<bool> ImportObj(MapId id);
    Task<bool> ExportPslm(MapId id);
    Task<bool> ImportPslm(MapId id);
    Task<bool> RevertModelToDefault(MapId id);
    Task<bool> ExportObj(GimmickObjectId id, int variant);
    string ResolveGimmickModelFilePath(GimmickObjectId id, int variant);
}
