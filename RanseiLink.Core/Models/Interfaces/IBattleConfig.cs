
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Types;

namespace RanseiLink.Core.Models.Interfaces;

public interface IBattleConfig : IDataWrapper, ICloneable<IBattleConfig>
{
    BattleVictoryConditionFlags VictoryCondition { get; set; }
    uint NumberOfTurns { get; set; }
    BattleVictoryConditionFlags DefeatCondition { get; set; }
    Rgb555 UpperAtmosphereColor { get; set; }
    Rgb555 MiddleAtmosphereColor { get; set; }
    Rgb555 LowerAtmosphereColor { get; set; }
    MapId MapId { get; set; }
}
