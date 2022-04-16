

namespace RanseiLink.Core.Models
{
    public class MoveAnimation : BaseDataWindow
    {
        public const int DataLength = 4;
        public MoveAnimation(byte[] data) : base(data, DataLength) { }
        public MoveAnimation() : this(new byte[DataLength]) { }

        public int UnknownA
        {
            get => GetUInt16(0);
            set => SetUInt16(0, (ushort)value);
        }

        public int UnknownB
        {
            get => GetUInt16(2);
            set => SetUInt16(2, (ushort)value);
        }

    }
}