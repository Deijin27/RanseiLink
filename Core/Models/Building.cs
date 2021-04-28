using Core.Enums;
using Core.Structs;

namespace Core.Models
{
    public class Building : BaseDataWindow
    {
        public const int DataLength = 0x24;
        public Building(byte[] data) : base(data, DataLength) { }
        public Building() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x13);
            set => SetPaddedUtf8String(0, 0x13, value);
        }

        public LocationId Location
        {
            get => (LocationId)GetByte(27);
        }
    }
}
