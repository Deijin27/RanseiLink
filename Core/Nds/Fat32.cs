using System;
using System.IO;

namespace Core.Nds
{
    public static class Fat32
    {
        public class Entry
        {
            public const int Length = 8;
            public Entry(uint start, uint end)
            {
                Start = start;
                End = end;
            }

            public readonly uint Start;
            public readonly uint End;

            public int GetLength()
            {
                return (int)(End - Start);
            }

            /// <summary>
            /// Gets the offset pair from the stream, progressing the current position in the stream by 8
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>

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
}
