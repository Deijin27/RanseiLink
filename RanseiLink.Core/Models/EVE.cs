using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Models
{
    public class EVE
    {
        public static void Unpack(string filePath, string outFolder = null)
        {
            var eve = new EVE(filePath);

            outFolder = outFolder ?? FileUtil.MakeUniquePath(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath)));
            Directory.CreateDirectory(outFolder);

            {
                string folderA = Path.Combine(outFolder, "A");
                Directory.CreateDirectory(folderA);
                int groupCount = 0;
                foreach (var eventGroup in eve.EventGroupsA)
                {
                    string eventGroupFolder = Path.Combine(folderA, groupCount++.ToString().PadLeft(4, '0'));
                    Directory.CreateDirectory(eventGroupFolder);
                    int eventCount = 0;
                    foreach (var e in eventGroup)
                    {
                        string baseFile = Path.Combine(eventGroupFolder, eventCount++.ToString().PadLeft(4, '0'));
                        string eventFile = baseFile + ".psle";
                        string unknownFile = baseFile + ".bin";
                        File.WriteAllBytes(eventFile, e.Item2.AllData);
                        File.WriteAllBytes(unknownFile, e.Item1);
                    }
                }
            }

            {
                string folderB = Path.Combine(outFolder, "B");
                Directory.CreateDirectory(folderB);
                int groupCount = 0;
                foreach (var eventGroup in eve.EventGroupsB)
                {
                    string eventGroupFolder = Path.Combine(folderB, groupCount++.ToString().PadLeft(4, '0'));
                    Directory.CreateDirectory(eventGroupFolder);
                    int eventCount = 0;
                    foreach (var e in eventGroup)
                    {
                        string baseFile = Path.Combine(eventGroupFolder, eventCount++.ToString().PadLeft(4, '0'));
                        string eventFile = baseFile + ".psle";
                        File.WriteAllBytes(eventFile, e.AllData);
                    }
                }
            }

            string headerInfo = Path.Combine(outFolder, "headerinfo.bin");
            using (var bw = new BinaryWriter(File.Create(headerInfo)))
            {
                bw.Write(eve.FileId);
                bw.Write(eve.HeaderUnknown);
            }
        }

        public static void Pack(string EveFolder, string outFile = null)
        {
            var eve = new EVE();

            string folderA = Path.Combine(EveFolder, "A");
            string[] foldersInA = Directory.GetDirectories(folderA);
            Array.Sort(foldersInA);
            foreach (var eventGroupFolder in foldersInA)
            {
                var lst = new List<(byte[], EVE.Event)>();
                string[] eventFiles = Directory.GetFiles(eventGroupFolder, "*.psle");
                Array.Sort(eventFiles);
                string[] binFiles = Directory.GetFiles(eventGroupFolder, "*.bin");
                Array.Sort(binFiles);
                if (eventFiles.Length != binFiles.Length)
                {
                    throw new Exception($"Different number of bin files to psle in '{eventGroupFolder}' ({eventFiles.Length}, {binFiles.Length})");
                }
                for (int i = 0; i < eventFiles.Length; i++)
                {
                    var eventFile = eventFiles[i];
                    var binFile = binFiles[i];
                    var e = new EVE.Event() { AllData = File.ReadAllBytes(eventFile) };
                    var bin = File.ReadAllBytes(binFile);
                    Debug.Assert(bin.Length == 0x40);
                    lst.Add((bin, e));
                }
                eve.EventGroupsA.Add(lst);
            }

            string folderB = Path.Combine(EveFolder, "B");
            string[] foldersInB = Directory.GetDirectories(folderB);
            Array.Sort(foldersInB);
            foreach (var eventGroupFolder in foldersInB)
            {
                var lst = new List<EVE.Event>();
                foreach (var eventFile in Directory.GetFiles(eventGroupFolder, "*.psle"))
                {
                    var e = new EVE.Event() { AllData = File.ReadAllBytes(eventFile) };
                    lst.Add(e);
                }
                eve.EventGroupsB.Add(lst);
            }

            string headerInfo = Path.Combine(EveFolder, "headerinfo.bin");
            using (var br = new BinaryReader(File.OpenRead(headerInfo)))
            {
                eve.FileId = br.ReadInt32();
                if (eve.FileId != 0x64 && eve.FileId != 0x65)
                {
                    throw new Exception("Unexpected File Id");
                }
                eve.HeaderUnknown = br.ReadInt32();
            }

            outFile = outFile ?? FileUtil.MakeUniquePath(EveFolder + ".eve");
            eve.Save(outFile);
        }

        public struct Header
        {
            public const int HeaderLength = 32;

            public const string MagicNumber = "LSP\x0";                         // 65.eve data
            public int FortyEight; // 0x48                                     // 0x48
            public int FileId; // file name is this in hex                     // 0x65
            public int NumGroups; // 0x1B                                      // 0x1B
            public int NumOffsets;                                             // 0x86
            public int TableBDataOffset; // last entry in offset table A       // 0x11B4C
            public int Unknown;                                                // 0x2C10
            public int FileLength;                                             // 0x375A8

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
                FileId = br.ReadInt32();
                NumGroups = br.ReadInt32();
                NumOffsets = br.ReadInt32();
                TableBDataOffset = br.ReadInt32();
                Unknown = br.ReadInt32();
                FileLength = br.ReadInt32();

                Console.WriteLine($"      FileId: 0x{FileId:X}");
                Console.WriteLine($"   NumGroups: 0x{NumGroups:X}");
                Console.WriteLine($"  NumOffsets: 0x{NumOffsets:X}");
                Console.WriteLine($"TableBOffset: 0x{TableBDataOffset:X}");
                Console.WriteLine($"     Unknown: 0x{Unknown:X}");
                Console.WriteLine($"  FileLength: 0x{FileLength:X}");
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.WriteMagicNumber(MagicNumber);
                bw.Write(FortyEight);
                bw.Write(FileId);
                bw.Write(NumGroups);
                bw.Write(NumOffsets);
                bw.Write(TableBDataOffset);
                bw.Write(Unknown);
                bw.Write(FileLength);
            }
        }

        private const int OffsetTableStart = 0x100;

        public EVE()
        {

        }
        public EVE(string file)
        {
            Console.WriteLine(file);
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                // Header ------------------------------------------------------------------------------------

                var header = new Header(br);
                FileId = header.FileId;
                HeaderUnknown = header.Unknown;

                Console.WriteLine("GroupItemCounts:");
                int[] groupItemCounts = new int[header.NumGroups];
                for (int i = 0; i < header.NumGroups; i++)
                {
                    groupItemCounts[i] = br.ReadInt32();
                    Console.WriteLine($"  0x{groupItemCounts[i]:X}");
                }

                // Offset Tables -------------------------------------------------------------------------------

                Console.WriteLine("OffsetTableA:");
                br.BaseStream.Position = OffsetTableStart;
                int[][] groupsA = new int[header.NumGroups][];
                for (int groupId = 0; groupId < header.NumGroups; groupId++)
                {
                    Console.WriteLine($"  Group {groupId} (numItems:{groupItemCounts[groupId]}):");
                    groupsA[groupId] = new int[groupItemCounts[groupId]];
                    for (int idWithinGroup = 0; idWithinGroup < groupItemCounts[groupId]; idWithinGroup++)
                    {
                        groupsA[groupId][idWithinGroup] = br.ReadInt32();
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
                int[][] groupsB = new int[header.NumGroups][];
                for (int groupId = 0; groupId < header.NumGroups; groupId++)
                {
                    Console.WriteLine($"  Group {groupId}:");
                    groupsB[groupId] = new int[groupItemCounts[groupId]];
                    for (int idWithinGroup = 0; idWithinGroup < groupItemCounts[groupId]; idWithinGroup++)
                    {
                        groupsB[groupId][idWithinGroup] = br.ReadInt32();
                        Console.WriteLine($"    0x{groupsB[groupId][idWithinGroup]:X} (id: {idWithinGroup})");
                    }
                }

                Debug.Assert(groupsB.Sum(i => i.Length) == header.NumOffsets);

                var end = br.ReadInt32();
                Console.WriteLine($"END OF OFFSETS VALUE = 0x{end:X}");

                // Data ---------------------------------------------------------------------------------------

                var startOffsetA = br.BaseStream.Position;
                for (int a = 0; a < groupsA.Length; a++)
                {
                    int[] group = groupsA[a];
                    var eveGroup = new List<(byte[], Event)>();
                    EventGroupsA.Add(eveGroup);
                    for (int i = 0; i < group.Length; i++)
                    {
                        var eventOffset = group[i];
                        br.BaseStream.Position = startOffsetA + eventOffset;
                        byte[] unknown = br.ReadBytes(0x40);
                        eveGroup.Add((unknown, new Event(br)));
                    }
                }

                var startOffsetB = br.BaseStream.Position;
                Console.WriteLine("START OFFSET B: 0x{0:X}", startOffsetB);
                var expectedStartOffsetB = startOffsetA + header.TableBDataOffset;
                Debug.Assert(startOffsetB == expectedStartOffsetB, $"Start offset of B is unexpected: 0x{startOffsetB:X} (expected: 0x{expectedStartOffsetB:X})");

                for (int b = 0; b < groupsB.Length; b++)
                {
                    int[] group = groupsB[b];
                    var eveGroup = new List<Event>();
                    EventGroupsB.Add(eveGroup);
                    for (int i = 0; i < group.Length; i++)
                    {
                        var eventOffset = group[i];
                        br.BaseStream.Position = startOffsetB + eventOffset;
                        eveGroup.Add(new Event(br));
                    }
                } 
            
                // ----------------------------------------------------------------------------
            }
        }

        public void Save(string file)
        {
            Console.WriteLine(file);
            using (var bw = new BinaryWriter(File.Create(file)))
            {
                // skip header until later ------------------------------------------------------------------------------------

                bw.Pad(Header.HeaderLength);

                if (EventGroupsA.Count != EventGroupsB.Count)
                {
                    throw new Exception($"Event groups A and B count don't match ({EventGroupsA.Count}, {EventGroupsB.Count})");
                }
                int numGroups = EventGroupsA.Count;

                // write the list containing the number of items in each subgroup ----------------------------------------------

                for (int i = 0; i < numGroups; i++)
                {
                    var groupA = EventGroupsA[i];
                    var groupB = EventGroupsB[i];
                    if (groupA.Count != groupB.Count)
                    {
                        throw new Exception($"Event group {i} does not contain the same number of events in A and B");
                    }
                    bw.Write(groupA.Count);
                }

                // skip offset table until later ------------------------------------------------------------------------------

                int numOffsets = EventGroupsA.Sum(i => i.Count);

                // the +1 is because there's an 'end' one too
                // *4 for each byte in an offset entry
                // *2 for the two tables A and B
                int dataStart = OffsetTableStart + (numOffsets + 1) * 4 * 2; 
                bw.BaseStream.Seek(dataStart, SeekOrigin.Begin);

                // write data, keep track of offsets ----------------------------------------------------------------------------

                List<List<int>> offsetsA = new List<List<int>>();
                long startOffsetA = bw.BaseStream.Position;
                foreach (var group in EventGroupsA)
                {
                    List<int> subgroupOffsets = new List<int>();
                    offsetsA.Add(subgroupOffsets);

                    foreach (var (b, e) in group)
                    {
                        subgroupOffsets.Add((int)(bw.BaseStream.Position - startOffsetA));
                        bw.Write(b);
                        e.WriteTo(bw);
                    }
                }

                List<List<int>> offsetsB = new List<List<int>>();
                long startOffsetB = bw.BaseStream.Position;
                int tableBDataOffset = (int)(startOffsetB - startOffsetA);
                foreach (var group in EventGroupsB)
                {
                    List<int> subgroupOffsets = new List<int>();
                    offsetsB.Add(subgroupOffsets);

                    foreach (var e in group)
                    {
                        subgroupOffsets.Add((int)(bw.BaseStream.Position - startOffsetB));
                        e.WriteTo(bw);
                    }
                }

                int dataEndB = (int)(bw.BaseStream.Position - startOffsetB);

                // pad the file to be divisible by 0x10
                while (bw.BaseStream.Length % 8 != 0)
                {
                    bw.Pad(1);
                }

                // write offsets ----------------------------------------------------------------------------------------------

                bw.BaseStream.Position = OffsetTableStart;
                foreach (var offsets in offsetsA)
                {
                    foreach (var o in offsets)
                    {
                        bw.Write(o);
                    }
                }
                bw.Write(tableBDataOffset);
                foreach (var offsets in offsetsB)
                {
                    foreach (var o in offsets)
                    {
                        bw.Write(o);
                    }
                }
                bw.Write(dataEndB);

                // write the header ----------------------------------------------------------------------------------------------
                var header = new Header()
                {
                    FortyEight = 0x48,
                    FileId = FileId,
                    Unknown = HeaderUnknown,
                    NumGroups = numGroups,
                    TableBDataOffset = tableBDataOffset,
                    FileLength = (int)bw.BaseStream.Length,
                    NumOffsets = numOffsets
                };
                bw.BaseStream.Position = 0;
                header.WriteTo(bw);
            }
        }

        public int FileId { get; set; }
        public int HeaderUnknown { get; set; }
        public List<List<(byte[], Event)>> EventGroupsA { get; set; } = new List<List<(byte[], Event)>>();
        public List<List<Event>> EventGroupsB { get; set; } = new List<List<Event>>();

        public class Event
        {
            public Header HeaderInstance { get; set; }
            public byte[] AllData { get; set; }

            public struct Header
            {
                public const string MagicNumber = "LSP\x0";                         // 65.eve data
                public int FortyEight; // 0x48 = length of header + footer
                public int Zero; // zero                    // 0x65
                public int TwentyEight; // 0x1B                           // 0x1B
                public int ThirtyThree;                                             // 0x86
                public int One; // last entry in offset table A       // 0x11B4C
                public int AboutFileLength;                                                // 0x2C10
                public int NotQuiteFileLength;                                             // 0x375A8

                public Header(BinaryReader br)
                {
                    Console.WriteLine($"       Start: 0x{br.BaseStream.Position:X} -------------------------------");
                    var magicNumber = br.ReadMagicNumber();
                    if (magicNumber != MagicNumber)
                    {
                        throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber}) at position 0x{br.BaseStream.Position:X}");
                    }
                    FortyEight = br.ReadInt32();
                    if (FortyEight != 0x48)
                    {
                        throw new InvalidDataException($"Unexpected data in eve");
                    }
                    Zero = br.ReadInt32();
                    TwentyEight = br.ReadInt32();
                    ThirtyThree = br.ReadInt32();
                    One = br.ReadInt32();
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

            public Event()
            {

            }
            public Event(BinaryReader br)
            {
                var startOffset = br.BaseStream.Position;
                var header = new Header(br);
                HeaderInstance = header;
                br.BaseStream.Position = startOffset;
                AllData = br.ReadBytes(header.AboutFileLength + 8);
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(AllData);
            }
        }
    }
}