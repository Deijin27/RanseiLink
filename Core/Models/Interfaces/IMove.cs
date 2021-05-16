using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IMove : IDataWrapper, ICloneable<IMove>
    {
        uint Accuracy { get; set; }
        MoveEffectId Effect0 { get; set; }
        uint Effect0Chance { get; set; }
        MoveEffectId Effect1 { get; set; }
        uint Effect1Chance { get; set; }
        MoveEffectId Effect2 { get; set; }
        uint Effect2Chance { get; set; }
        MoveEffectId Effect3 { get; set; }
        uint Effect3Chance { get; set; }
        MoveMovementFlags MovementFlags { get; set; }
        string Name { get; set; }
        uint Power { get; set; }
        MoveRangeId Range { get; set; }
        TypeId Type { get; set; }
    }
}