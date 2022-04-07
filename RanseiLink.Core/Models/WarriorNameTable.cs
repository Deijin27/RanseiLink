using System;

namespace RanseiLink.Core.Models;

public class WarriorNameTable : BaseDataWindow
{
    public const int DataLength = 0x9D8;
    public const int EntryLength = 0xC;
    public const int EntryCount = 0xD2;
    public WarriorNameTable(byte[] data) : base(data, DataLength, true) { }
    public WarriorNameTable() : this(new byte[DataLength]) { }

    public string GetEntry(uint id)
    {
        ValidateIdWithThrow(id);
        return GetPaddedUtf8String(EntryLength * (int)id, 10);
    }

    public void SetEntry(uint id, string value)
    {
        ValidateIdWithThrow(id);
        SetPaddedUtf8String(EntryLength * (int)id, 10, value);
    }

    private void ValidateIdWithThrow(uint id)
    {
        if (id >= EntryCount)
        {
            throw new ArgumentException($"{nameof(id)} of entry requested from {nameof(WarriorNameTable)} must be less than {EntryCount}");
        }
    }

    public bool ValidateId(uint id)
    {
        return id < EntryCount;
    }

}
