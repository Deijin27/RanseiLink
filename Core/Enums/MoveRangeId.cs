
namespace Core.Enums
{
    /// <summary>
    /// UInt5.
    /// 
    /// This determines where the attack affects.
    /// The movement of user or target happens *afterwards*.
    /// </summary>
    public enum MoveRangeId : byte
    {
        NoRange,
        Ahead1Tile,
        Column2Tiles,
        Column3Tiles,
        DiamondAdjacent,
        RingAdjacent,
        Row,
        Unused_0,
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
    }
}
