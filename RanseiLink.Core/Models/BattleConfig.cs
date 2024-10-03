using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;

namespace RanseiLink.Core.Models;

public partial class BattleConfig
{
    public MapId MapId
    {
        get => new(Map, MapVariant);
        set
        {
            Map = value.Map;
            MapVariant = value.Variant;
        }
    }
}