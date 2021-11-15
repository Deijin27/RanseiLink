using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IScenarioKingdom : IDataWrapper, ICloneable<IScenarioKingdom>
{
    uint GetBattlesToUnlock(KingdomId kingdom);
    void SetBattlesToUnlock(KingdomId kingdom, uint value);
}
