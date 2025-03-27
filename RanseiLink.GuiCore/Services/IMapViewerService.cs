using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;

namespace RanseiLink.GuiCore.Services;
public interface IMapViewerService
{
    Task ShowDialog(MapId id);
    Task ShowDialog(BattleConfigId id);
    Task ShowDialog(GimmickObjectId id, int variant);
}
