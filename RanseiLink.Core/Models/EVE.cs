using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Models
{
    public class EVE
    {
        public struct Header
        {
            public const string MagicNumber = "LSP\x0";                         // 65.eve data
            public uint FortyEight; // 0x48                                     // 0x48
            public uint FileId; // file name is this in hex                     // 0x65
            public int NumGroups; // 0x1B                           // 0x1B
            public uint NumOffsets;                                             // 0x86
            public uint TableBDataOffset; // last entry in offset table A       // 0x11B4C
            public uint Unknown;                                                // 0x2C10
            public uint FileLength;                                             // 0x375A8

            public Header(BinaryReader br)
            {
                var magicNumber = br.ReadMagicNumber();
                if (magicNumber != MagicNumber)
                {
                    throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                }
                FortyEight = br.ReadUInt32();
                if (FortyEight != 0x48)
                {
                    throw new InvalidDataException($"Unexpected data in eve");
                }
                FileId = br.ReadUInt32();
                NumGroups = br.ReadInt32();
                NumOffsets = br.ReadUInt32();
                TableBDataOffset = br.ReadUInt32();
                Unknown = br.ReadUInt32();
                FileLength = br.ReadUInt32();

                Console.WriteLine($"      FileId: 0x{FileId:X}");
                Console.WriteLine($"   NumGroups: 0x{NumGroups:X}");
                Console.WriteLine($"  NumOffsets: 0x{NumOffsets:X}");
                Console.WriteLine($"TableBOffset: 0x{TableBDataOffset:X}");
                Console.WriteLine($"     Unknown: 0x{Unknown:X}");
                Console.WriteLine($"  FileLength: 0x{FileLength:X}");
            }
        }

        private const int OffsetTableStart = 0x100;

        public EVE(string file)
        {
            Console.WriteLine(file);
            using (var br = new BinaryReader(File.OpenRead(file)))
            {

                var header = new Header(br);

                Console.WriteLine("GroupItemCounts:");
                int[] groupItemCounts = new int[header.NumGroups];
                long unknownsSum = 0;
                for (int i = 0; i < header.NumGroups; i++)
                {
                    groupItemCounts[i] = br.ReadInt32();
                    Console.WriteLine($"  0x{groupItemCounts[i]:X}");
                    unknownsSum += groupItemCounts[i];
                }

                Console.WriteLine("OffsetTableA:");
                br.BaseStream.Position = OffsetTableStart;
                uint[][] groupsA = new uint[header.NumGroups][];
                for (int groupId = 0; groupId < header.NumGroups; groupId++)
                {
                    Console.WriteLine($"  Group {groupId} (numItems:{groupItemCounts[groupId]}):");
                    groupsA[groupId] = new uint[groupItemCounts[groupId]];
                    for (int idWithinGroup = 0; idWithinGroup < groupItemCounts[groupId]; idWithinGroup++)
                    {
                        groupsA[groupId][idWithinGroup] = br.ReadUInt32();
                        Console.WriteLine($"    0x{groupsA[groupId][idWithinGroup]:X} (id: {idWithinGroup})");
                    }
                }

                Debug.Assert(groupsA.Sum(i => i.Length) == header.NumOffsets);

                var shift = br.ReadUInt32();
                if (shift != header.TableBDataOffset)
                {
                    throw new InvalidDataException("Listed offset doesnt match earlier value");
                }


                Console.WriteLine("OffsetTableB:");
                uint[][] groupsB = new uint[header.NumGroups][];
                for (int groupId = 0; groupId < header.NumGroups; groupId++)
                {
                    Console.WriteLine($"  Group {groupId}:");
                    groupsB[groupId] = new uint[groupItemCounts[groupId]];
                    for (int idWithinGroup = 0; idWithinGroup < groupItemCounts[groupId]; idWithinGroup++)
                    {
                        groupsB[groupId][idWithinGroup] = br.ReadUInt32();
                        Console.WriteLine($"    0x{groupsB[groupId][idWithinGroup]:X} (id: {idWithinGroup})");
                    }
                }

                Debug.Assert(groupsB.Sum(i => i.Length) == header.NumOffsets);

                var end = br.ReadUInt32();

                Console.WriteLine($"Position: 0x{br.BaseStream.Position:X}");

                byte[] unknownBytes = br.ReadBytes(0x40);

                var itemStartOffset = br.BaseStream.Position;

                EventGroupsA = new List<List<Event>>();
                foreach (uint[] group in groupsA)
                {
                    var eveGroup = new List<Event>();
                    EventGroupsA.Add(eveGroup);
                    foreach (uint eventOffset in group)
                    {
                        br.BaseStream.Position = itemStartOffset + eventOffset;
                        eveGroup.Add(new Event(br));
                    }
                }

                EventGroupsB = new List<List<Event>>();
                foreach (uint[] group in groupsA)
                {
                    var eveGroup = new List<Event>();
                    EventGroupsB.Add(eveGroup);
                    foreach (uint eventOffset in group)
                    {
                        br.BaseStream.Position = itemStartOffset + eventOffset;
                        eveGroup.Add(new Event(br));
                    }
                } }
        }

        public List<List<Event>> EventGroupsA { get; set; }
        public List<List<Event>> EventGroupsB { get; set; }

        public class Event
        {
            public Header HeaderInstance { get; set; }
            public byte[] AllData { get; set; }

            public struct Header
            {
                public const string MagicNumber = "LSP\x0";                         // 65.eve data
                public int FortyEight; // 0x48 = length of header + footer
                public uint Zero; // zero                    // 0x65
                public int TwentyEight; // 0x1B                           // 0x1B
                public uint ThirtyThree;                                             // 0x86
                public uint One; // last entry in offset table A       // 0x11B4C
                public int AboutFileLength;                                                // 0x2C10
                public int NotQuiteFileLength;                                             // 0x375A8

                public Header(BinaryReader br)
                {
                    var magicNumber = br.ReadMagicNumber();
                    if (magicNumber != MagicNumber)
                    {
                        throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                    }
                    FortyEight = br.ReadInt32();
                    if (FortyEight != 0x48)
                    {
                        throw new InvalidDataException($"Unexpected data in eve");
                    }
                    Zero = br.ReadUInt32();
                    TwentyEight = br.ReadInt32();
                    ThirtyThree = br.ReadUInt32();
                    One = br.ReadUInt32();
                    AboutFileLength = br.ReadInt32();
                    NotQuiteFileLength = br.ReadInt32();

                    Console.WriteLine($"      FileId: 0x{Zero:X}");
                    Console.WriteLine($"   NumGroups: 0x{TwentyEight:X}");
                    Console.WriteLine($"  NumOffsets: 0x{ThirtyThree:X}");
                    Console.WriteLine($"TableBOffset: 0x{One:X}");
                    Console.WriteLine($"     Unknown: 0x{AboutFileLength:X}");
                    Console.WriteLine($"  FileLength: 0x{NotQuiteFileLength:X}");


                }
            }

            public Event(BinaryReader br)
            {
                var startOffset = br.BaseStream.Position;
                var header = new Header(br);
                HeaderInstance = header;
                br.BaseStream.Position = startOffset;
                AllData = br.ReadBytes(header.AboutFileLength + header.FortyEight);
            }
        }
    }
}