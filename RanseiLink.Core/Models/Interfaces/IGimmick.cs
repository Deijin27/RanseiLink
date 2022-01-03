
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IGimmick : IDataWrapper, ICloneable<IGimmick>
{
    string Name { get; set; }
    TypeId AttackType { get; set; }
    TypeId DestroyType { get; set; }
    MoveAnimationId Animation1 { get; set; }
    MoveAnimationId Animation2 { get; set; }
    GimmickRangeId Range { get; set; }
    GimmickImageId Image { get; set; }
    GimmickSpriteId State1Sprite { get; set; }
    GimmickSpriteId State2Sprite { get; set; }
    MoveEffectId Effect { get; set; }
}
