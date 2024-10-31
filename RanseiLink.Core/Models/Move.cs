namespace RanseiLink.Core.Models;

public partial class Move
{
    public int StarCount
    {
        get
        {
            return Power switch
            {
                > 50 => 5,
                > 40 => 4,
                > 30 => 3,
                > 20 => 2,
                > 0 => 1,
                _ => 0
            };
        }
    }
}
