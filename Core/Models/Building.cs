using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class Building : BaseDataWindow, IBuilding
    {
        public const int DataLength = 0x24;
        public Building(byte[] data) : base(data, DataLength) { }
        public Building() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x13);
            set => SetPaddedUtf8String(0, 0x13, value);
        }

        public KingdomId Kingdom
        {
            get => (KingdomId)GetUInt32(6, 5, 24);
            set => SetUInt32(6, 5, 24, (uint)value);
        }

        public IBuilding Clone()
        {
            return new Building((byte[])Data.Clone());
        }
    }
}
