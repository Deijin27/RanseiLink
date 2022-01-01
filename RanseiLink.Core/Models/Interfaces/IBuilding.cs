using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IBuilding : IDataWrapper, ICloneable<IBuilding>
{
    KingdomId Kingdom { get; set; }
    string Name { get; set; }
    BattleConfigId BattleConfig1 { get; set; }
    BattleConfigId BattleConfig2 { get; set; }
    BattleConfigId BattleConfig3 { get; set; }
    BuildingSpriteId Sprite1 { get; set; }
    BuildingSpriteId Sprite2 { get; set; }
    BuildingSpriteId Sprite3 { get; set; }
    BuildingFunctionId Function { get; set; }
}
