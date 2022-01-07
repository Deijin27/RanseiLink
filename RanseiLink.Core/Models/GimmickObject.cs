using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class GimmickObject : BaseDataWindow, IGimmickObject
{
    public const int DataLength = 0x4;
    public GimmickObject(byte[] data) : base(data, DataLength) { }
    public GimmickObject() : base(new byte[DataLength], DataLength) { }

    public IGimmickObject Clone()
    {
        return new GimmickObject((byte[])Data.Clone());
    }
}
