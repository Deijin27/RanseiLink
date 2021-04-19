using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ransei.Nds
{
    public static class Nds
    {
        public static Fat32.Entry GetEntryFromPath(BinaryReader stream, string path)
        {
            uint entryIndex = NdsNameTable.GetFatEntryIndex(stream, path);
            return NdsFileAllocationTable.GetEntry(stream, entryIndex);
        }

        /// <summary>
        /// Make a copy of a file within the nds file system to the destination on the computer file system.
        /// </summary>
        /// <param name="source">Nds stream</param>
        /// <param name="path">path to file within Nds file system</param>
        /// <param name="destination">destination file on computer</param>
        public static void CopyExtractFile(BinaryReader source, string path, string destination)
        {
            var entry = GetEntryFromPath(source, path);
            source.BaseStream.Position = entry.Start;
            File.WriteAllBytes(destination, source.ReadBytes(entry.GetLength()));
        }

        public static void InsertFixedLengthFile(Stream nds, string path, string source)
        {
            var entry = GetEntryFromPath(new BinaryReader(nds), path);
            nds.Position = entry.Start;
            byte[] sourceData = File.ReadAllBytes(source);
            nds.Write(sourceData, 0, entry.GetLength());
        }
    }
}
