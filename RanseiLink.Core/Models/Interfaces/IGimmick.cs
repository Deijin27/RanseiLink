
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
    GimmickObjectId State1Object { get; set; }
    GimmickObjectId State2Object { get; set; }
    MoveEffectId Effect { get; set; }
}
