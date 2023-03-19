using System.IO;

namespace RanseiLink.Core.RomFs;

public static class Fat32
{
    public class Entry
    {

        public Entry(uint start, uint end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Length of entry itself
        /// </summary>
        public const int Length = 8;

        public uint Start { get; }
        public uint End { get; }

        /// <summary>
        /// Length of data block that entry points to
        /// </summary>
        public int GetLength()
        {
            return (int)(End - Start);
        }

        public bool IsDefault => Start == uint.MaxValue && End == uint.MaxValue;
    }

    public static class FileAllocationTable
    {
        public static Entry ReadEntry(BinaryReader stream, long tableOffset, uint entryIndex)
        {
            stream.BaseStream.Position = tableOffset + entryIndex * Entry.Length;
            return ReadEntryFromCurrentOffset(stream);
        }

        static Entry ReadEntryFromCurrentOffset(BinaryReader stream)
        {
            return new Entry(stream.ReadUInt32(), stream.ReadUInt32());
        }

        public static void WriteEntry(BinaryWriter stream, long tableOffset, uint entryIndex, Entry entry)
        {
            stream.BaseStream.Position = tableOffset + entryIndex * Entry.Length;
            stream.Write(entry.Start);
            stream.Write(entry.End);
        }

        public static Entry[] AllEntries(BinaryReader stream, long tableOffset, uint entryCount)
        {
            var entries = new Entry[entryCount];
            stream.BaseStream.Position = tableOffset;
            for (int i = 0; i < entryCount; i++)
            {
                entries[i] = ReadEntryFromCurrentOffset(stream);
            }

            return entries;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bounds"></param>
    /// <param name="offsetZeroStreamPosition">Position in stream equivalent to a zero value fro an offset in a FatEntry</param>
    /// <returns></returns>
    public static byte[] ReadCluster(BinaryReader stream, Entry bounds, long offsetZeroStreamPosition = 0)
    {
        stream.BaseStream.Position = offsetZeroStreamPosition + bounds.Start;
        return stream.ReadBytes(bounds.GetLength());
    }
}