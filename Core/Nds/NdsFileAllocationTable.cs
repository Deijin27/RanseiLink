using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ransei.Nds
{
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

    }
}
