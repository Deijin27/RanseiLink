using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IScenarioKingdom : IDataWrapper, ICloneable<IScenarioKingdom>
{
    uint GetArmy(KingdomId kingdom);
    void SetArmy(KingdomId kingdom, uint armyId);
}
