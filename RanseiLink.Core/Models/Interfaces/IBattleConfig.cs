
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IBattleConfig : IDataWrapper, ICloneable<IBattleConfig>
{
    uint Environment { get; set; }
    uint EnvironmentVariant { get; set; }
    BattleVictoryConditionFlags VictoryCondition { get; set; }
    uint NumberOfTurns { get; set; }
    BattleVictoryConditionFlags DefeatCondition { get; set; }
}
