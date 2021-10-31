using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models
{
    public class Gimmick : BaseDataWindow, IGimmick
    {
        public const int DataLength = 0x28;
        public Gimmick(byte[] data) : base(data, DataLength) { }
        public Gimmick() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 16);
            set => SetPaddedUtf8String(0, 16, value);
        }

        public IGimmick Clone()
        {
            return new Gimmick((byte[])Data.Clone());
        }
    }
}
