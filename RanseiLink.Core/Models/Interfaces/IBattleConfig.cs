
using RanseiLink.Core.Enums;
using RanseiLink.Core.Types;

namespace RanseiLink.Core.Models.Interfaces;

public interface IBattleConfig : IDataWrapper, ICloneable<IBattleConfig>
{
    uint Environment { get; set; }
    uint EnvironmentVariant { get; set; }
    BattleVictoryConditionFlags VictoryCondition { get; set; }
    uint NumberOfTurns { get; set; }
    BattleVictoryConditionFlags DefeatCondition { get; set; }
    Rgb555 UpperAtmosphereColor { get; set; }
    Rgb555 MiddleAtmosphereColor { get; set; }
    Rgb555 LowerAtmosphereColor { get; set; }
}
