using Core.Enums;
using Core.Structs;

namespace Core.Models
{
    public class Gimmick : BaseDataWindow
    {
        public const int DataLength = 0x28;
        public Gimmick(byte[] data) : base(data, DataLength) { }
        public Gimmick() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x0F);
            set => SetPaddedUtf8String(0, 0x0F, value);
        }
    }
}
