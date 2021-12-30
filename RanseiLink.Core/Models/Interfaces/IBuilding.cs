using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IBuilding : IDataWrapper, ICloneable<IBuilding>
{
    KingdomId Kingdom { get; set; }
    string Name { get; set; }
    BattleMapId BattleMap1 { get; set; }
    BattleMapId BattleMap2 { get; set; }
    BattleMapId BattleMap3 { get; set; }
}
