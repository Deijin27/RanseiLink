using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces
{
    public interface IMove : IDataWrapper, ICloneable<IMove>
    {
        uint Accuracy { get; set; }
        MoveEffectId Effect1 { get; set; }
        uint Effect1Chance { get; set; }
        MoveEffectId Effect2 { get; set; }
        uint Effect2Chance { get; set; }
        MoveEffectId Effect3 { get; set; }
        uint Effect3Chance { get; set; }
        MoveEffectId Effect4 { get; set; }
        uint Effect4Chance { get; set; }
        MoveMovementFlags MovementFlags { get; set; }
        string Name { get; set; }
        uint Power { get; set; }
        MoveRangeId Range { get; set; }
        TypeId Type { get; set; }
        MoveAnimationId StartupAnimation { get; set; }
        MoveAnimationId ProjectileAnimation { get; set; }
        MoveAnimationId ImpactAnimation { get; set; }
        MoveAnimationTargetId AnimationTarget1 { get; set; }
        MoveAnimationTargetId AnimationTarget2 { get; set; }
        MoveMovementAnimationId MovementAnimation { get; set; }
    }
}