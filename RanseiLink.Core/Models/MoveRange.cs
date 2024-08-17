

namespace RanseiLink.Core.Models;

public class MoveRange : BaseDataWindow
{
    public const int DataLength = 4;
    public MoveRange(byte[] data) : base(data, DataLength) { }
    public MoveRange() : base(new byte[DataLength], DataLength) { }


    public bool GetInRange(int position)
    {
        return GetInt(0, position, 1) == 1;
    }

    public void SetInRange(int position, bool isInRange)
    {
        SetInt(0, position, 1, isInRange ? 1 : 0);
    }

}