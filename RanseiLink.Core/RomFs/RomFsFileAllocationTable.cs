namespace RanseiLink.Core.RomFs;

public static class RomFsFileAllocationTable
{
    public static Fat32.Entry GetEntry(BinaryReader stream, long fatStartOffset, uint entryIndex)
    {
        return Fat32.FileAllocationTable.ReadEntry(stream, fatStartOffset, entryIndex);
    }

    public static void SetEntry(BinaryWriter stream, long fatStartOffset, uint entryIndex, Fat32.Entry entry)
    {
        Fat32.FileAllocationTable.WriteEntry(stream, fatStartOffset, entryIndex, entry);
    }
}