namespace RanseiLink.Core.Models
{
    public class GimmickObject : BaseDataWindow
    {
        public const int DataLength = 0x4;
        public GimmickObject(byte[] data) : base(data, DataLength) { }
        public GimmickObject() : base(new byte[DataLength], DataLength) { }

    }
}