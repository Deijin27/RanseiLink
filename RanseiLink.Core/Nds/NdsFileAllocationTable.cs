using System.IO;

namespace RanseiLink.Core.Nds;

public static class NdsFileAllocationTable
{
    public static long GetStartOffset(BinaryReader nds)
    {
        nds.BaseStream.Position = 0x48;
        return nds.ReadUInt32();
    }

    public static Fat32.Entry GetEntry(BinaryReader stream, long fatStartOffset, uint entryIndex)
    {
        return Fat32.FileAllocationTable.ReadEntry(stream, fatStartOffset, entryIndex);
    }

    public static Fat32.Entry GetEntry(BinaryReader stream, uint entryIndex)
    {
        return GetEntry(stream, GetStartOffset(stream), entryIndex);
    }

    public static void SetEntry(BinaryWriter stream, long fatStartOffset, uint entryIndex, Fat32.Entry entry)
    {
        Fat32.FileAllocationTable.WriteEntry(stream, fatStartOffset, entryIndex, entry);
    }
}
