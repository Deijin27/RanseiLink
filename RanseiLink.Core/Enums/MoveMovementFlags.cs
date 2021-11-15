using System;

namespace RanseiLink.Core.Enums;

[Flags]
public enum MoveMovementFlags : uint
{
    MovementOrKnockback = 0b_0000_0001,
    InvertMovementDirection = 0b_0000_0100,
    DoubleMovementDistance = 0b_0000_1000,

    Knockback = MovementOrKnockback | InvertMovementDirection
}
