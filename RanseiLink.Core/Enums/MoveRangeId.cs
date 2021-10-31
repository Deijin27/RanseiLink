
namespace RanseiLink.Core.Enums
{
    /// <summary>
    /// UInt5.
    /// 
    /// This determines where the attack affects.
    /// The movement of user or target happens *afterwards*.
    /// </summary>
    public enum MoveRangeId : uint
    {
        NoRange,
        Ahead1Tile,
        Column2Tiles,
        Column3Tiles,
        DiamondAdjacent,
        RingAdjacent,
        Row,
        TwoLittleUnusedDots,
        Chevron,
        Plus,
        Cross,
        Ahead2Tiles,
        Ahead3Tiles,
        Diamond2Ahead,
        Ring2Ahead,
        Row2Ahead,
        TwoRows2Ahead,
        Column2Tiles2Ahead,
        Cross2Ahead,
        Dai,
        TShape,
        TwoRows,
        Unused_1,
        Unused_2,
        Unused_3,
        Unused_4,
        Unused_5,
        Unused_6,
        Unused_7,
        Unused_8,
    }
}
