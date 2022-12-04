using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class ScenarioBuilding : BaseDataWindow
    {
        public const int DataLength = 0xEE;
        /// <summary>
        /// Slots per kingdom
        /// </summary>
        public const int SlotCount = 7;
        private const int _bytesPerSlot = 2;
        public ScenarioBuilding(byte[] data) : base(data, DataLength, false)
        {
        }

        private int GetBaseOffset(int kingdom, int slot) => kingdom * SlotCount * _bytesPerSlot + slot * _bytesPerSlot;

        public int GetInitialExp(int kingdom, int slot)
        {
            return GetByte(GetBaseOffset(kingdom, slot));
        }

        public int GetUnknown2(int kingdom, int slot)
        {
            return GetByte(GetBaseOffset(kingdom, slot) + 1);
        }

        public void SetInitialExp(int kingdom, int slot, int value)
        {
            SetByte(GetBaseOffset(kingdom, slot), (byte)value);
        }

        public void SetUnknown2(int kingdom, int slot, int value)
        {
            SetByte(GetBaseOffset(kingdom, slot) + 1, (byte)value);
        }

        public int GetInitialExp(KingdomId kingdom, int slot) => GetInitialExp((int)kingdom, slot);
        public int GetUnknown2(KingdomId kingdom, int slot) => GetUnknown2((int)kingdom, slot);
        public void SetInitialExp(KingdomId kingdom, int slot, int value) => SetInitialExp((int)kingdom, slot, value);
        public void SetUnknown2(KingdomId kingdom, int slot, int value) => SetUnknown2((int)kingdom, slot, value);
    }
}
