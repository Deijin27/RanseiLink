namespace RanseiLink.Core.Models;
public partial class MoveRange
{
    private static readonly int[][] __map = [
        [28, 29, 25, 26, 27],
        [23, 24, 09, 10, 11],
        [22, 08, 01, 02, 12],
        [21, 07, 00, 03, 13],
        [20, 06, 05, 04, 14],
        [19, 18, 17, 16, 15]
        ];

    public static int GetBitIndex(int row, int column)
    {
        return __map[row][column];
    }

    public bool GetInRange(int row, int column)
    {
        return GetInRange(GetBitIndex(row, column));
    }

    public void SetInRange(int row, int column, bool value)
    {
        SetInRange(GetBitIndex(row, column), value);
    }
}
