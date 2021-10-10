using Core.Models.Interfaces;

namespace Core.Models
{
    public class BaseWarrior : BaseDataWindow, IBaseWarrior
    {
        public const int DataLength = 0x14;
        public BaseWarrior(byte[] data) : base(data, DataLength) { }
        public BaseWarrior() : base(new byte[DataLength], DataLength) { }

        public uint WarriorName
        {
            get => GetUInt32(0, 8, 17);
            set => SetUInt32(0, 8, 17, value);
        }

        public IBaseWarrior Clone()
        {
            return new BaseWarrior((byte[])Data.Clone());
        }
    }
}
