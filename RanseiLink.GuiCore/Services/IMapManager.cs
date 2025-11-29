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
    Task<bool> ExportObj(GimmickObjectId id, int[] variant);
    Task<bool> ImportObj_TexturesOnly(GimmickObjectId id, int variant);
    Task<bool> RevertModelToDefault(GimmickObjectId id, int variant);
    bool IsOverriden(GimmickObjectId id, int variant);
}
