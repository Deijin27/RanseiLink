using Core.Models.Interfaces;

namespace Core.Models
{
    public class MoveRange : BaseDataWindow, IMoveRange
    {
        public const int DataLength = 4;
        public MoveRange(byte[] data) : base(data, DataLength) { }
        public MoveRange() : base(new byte[DataLength], DataLength) { }


        public bool GetInRange(int position)
        {
            return GetUInt32(0, 1, position) == 1;
        }

        public void SetInRange(int position, bool isInRange)
        {
            SetUInt32(0, 1, position, isInRange ? 1u : 0u);
        }
    }
}
