using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class AttackRange : BaseDataWindow, IAttackRange
{
    public const int DataLength = 4;
    public AttackRange(byte[] data) : base(data, DataLength) { }
    public AttackRange() : base(new byte[DataLength], DataLength) { }


    public bool GetInRange(int position)
    {
        return GetUInt32(0, 1, position) == 1;
    }

    public void SetInRange(int position, bool isInRange)
    {
        SetUInt32(0, 1, position, isInRange ? 1u : 0u);
    }

    public IAttackRange Clone()
    {
        return new AttackRange((byte[])Data.Clone());
    }
}
