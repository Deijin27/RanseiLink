using RanseiLink.Core.Models.Interfaces;
using System;

namespace RanseiLink.Core.Models
{
    public class WarriorNameTable : BaseDataWindow, IWarriorNameTable
    {
        public const int DataLength = 0x9D8;
        public const int EntryLength = 0xC;
        public const int EntryCount = 0xD2;
        public WarriorNameTable(byte[] data) : base(data, DataLength, true) { }
        public WarriorNameTable() : this(new byte[DataLength]) { }

        public string GetEntry(uint id)
        {
            ValidateId(id);
            return GetPaddedUtf8String(EntryLength * (int)id, 10);
        }

        public void SetEntry(uint id, string value)
        {
            ValidateId(id);
            SetPaddedUtf8String(EntryLength * (int)id, 10, value);
        }

        private void ValidateId(uint id)
        {
            if (id >= EntryCount)
            {
                throw new ArgumentException($"{nameof(id)} of entry requested from {nameof(WarriorNameTable)} must be less than {EntryCount}");
            }
        }

        public IWarriorNameTable Clone()
        {
            return new WarriorNameTable((byte[])Data.Clone());
        }
    }
}
