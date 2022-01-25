
using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Models.Interfaces;

public interface IBattleConfig : IDataWrapper, ICloneable<IBattleConfig>
{
    BattleVictoryConditionFlags VictoryCondition { get; set; }
    uint NumberOfTurns { get; set; }
    BattleVictoryConditionFlags DefeatCondition { get; set; }
    Rgb15 UpperAtmosphereColor { get; set; }
    Rgb15 MiddleAtmosphereColor { get; set; }
    Rgb15 LowerAtmosphereColor { get; set; }
    MapId MapId { get; set; }
}
