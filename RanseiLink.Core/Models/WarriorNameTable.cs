namespace RanseiLink.Core.Models
{
    public class WarriorNameTable : BaseDataWindow
    {
        public const int DataLength = 0x9D8;
        public const int EntryLength = 0xC;
        public const int EntryCount = 0xD2;
        public WarriorNameTable(byte[] data) : base(data, DataLength, true) { }
        public WarriorNameTable() : this(new byte[DataLength]) { }

        public string GetEntry(int id)
        {
            if (!ValidateId(id))
            {
                return "";
            }
            return GetPaddedUtf8String(EntryLength * (int)id, 10);
        }

        public void SetEntry(int id, string value)
        {
            if (!ValidateId(id))
            {
                return;
            }
            SetPaddedUtf8String(EntryLength * (int)id, 10, value);
        }

        private void ValidateIdWithThrow(int id)
        {
            if (id >= EntryCount || id < 0)
            {
                throw new ArgumentException($"{nameof(id)} of entry requested from {nameof(WarriorNameTable)} must be less than {EntryCount}");
            }
        }

        public bool ValidateId(int id)
        {
            return id < EntryCount;
        }

    }
}