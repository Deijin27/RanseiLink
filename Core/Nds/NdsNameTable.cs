using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ransei.Nds
{
    public static class NdsNameTable
    {
        public class FolderAllocation
        {
            public const int Length = 8;
            public FolderAllocation(byte[] data)
            {
                if (data.Length != Length)
                {
                    throw new Exception($"{nameof(NdsNameTable)}.{nameof(FolderAllocation)} must have an array passed to its constructor of length {Length}");
                }
                Data = data;
            }

            public byte[] Data { get; }

            public int Offset
            {
                get => BitConverter.ToInt32(Data, 0);
                set => BitConverter.GetBytes(value).CopyTo(Data, 0);
            }

            public ushort FatTopFileId
            {
                get => BitConverter.ToUInt16(Data, 4);
                set => BitConverter.GetBytes(value).CopyTo(Data, 4);
            }

            public FolderIndex FolderIndex
            {
                get => new FolderIndex(BitConverter.ToUInt16(Data, 6));
                set => BitConverter.GetBytes(value.ToUshort()).CopyTo(Data, 6);
            }
        }

        public class FileOrFolderName
        {

            public bool IsFolder { get; set; }
            public byte[] Name { get; set; }
            public FolderIndex ContentsIndexIfFolder { get; set; }
        }

        public readonly struct NameHeader
        {
            public NameHeader(byte rawData)
            {
                Length = new UInt7(rawData & 0b_0111_1111);
                IsFolder = (rawData & boolMark) == boolMark;
            }

            public NameHeader(UInt7 uint7, bool bool_)
            {
                Length = uint7;
                IsFolder = bool_;
            }

            const int boolMark = 0b_1000_0000;
            public readonly bool IsFolder;
            public readonly UInt7 Length;

            public byte ToByte()
            {
                return (byte)(Length.ToInt32() | (IsFolder ? boolMark : 0));
            }
        }

        public readonly struct UInt7
        {
            public UInt7(int value)
            {
                if (value > 127 && value >= 0)
                {
                    throw new Exception($"Value converted into a {nameof(UInt7)} must be between 0 and 127");
                }
                _value = value;
            }

            readonly int _value;

            public int ToInt32()
            {
                return _value;
            }
        }

        public readonly struct FolderIndex
        {
            public FolderIndex(ushort rawData)
            {
                Index = (byte)rawData;

                var sh = rawData >> 8;

                switch (sh)
                {
                    case IsRootFolderMarker:
                        Bool = true;
                        break;
                    case NotRootFolderMarker:
                        Bool = false;
                        break;
                    default:
                        throw new Exception($"Unexpected value (Index={Index}) (rawData={rawData:x}) [Bool={rawData >> 8:x}] (not 00 or F0) in FolderIndex Bool");
                };
            }
            const byte IsRootFolderMarker = 0x00;
            const byte NotRootFolderMarker = 0xF0;

            public readonly bool Bool;
            public readonly byte Index;

            public ushort ToUshort()
            {
                return (ushort)(Index | ((Bool ? IsRootFolderMarker : NotRootFolderMarker) << 8));
            }
        }

        public static long GetStartOffset(BinaryReader nds) 
        {
            nds.BaseStream.Position = 0x40;
            return nds.ReadUInt32();
        }


        public static FolderAllocation GetRootFolderAllocationData(BinaryReader stream, long startOffset)
        {
            return GetAllocationData(stream, startOffset, new FolderIndex(0x0000));
        }

        public static List<FileOrFolderName> GetRootFolderContents(BinaryReader stream, long startOffset)
        {
            var alloc = GetRootFolderAllocationData(stream, startOffset);
            return GetContents(stream, startOffset, alloc);
        }

        public static FolderAllocation GetAllocationData(BinaryReader stream, long startOffset, FolderIndex folderIndex)
        {
            // set the stream Position to the start of the requested folder's allocation info
            stream.BaseStream.Position = startOffset + folderIndex.Index * FolderAllocation.Length;

            // read the allocation info
            return new FolderAllocation(stream.ReadBytes(FolderAllocation.Length));
        }

        public static List<FileOrFolderName> GetContents(BinaryReader stream, long startOffset, FolderAllocation alloc)
        {
            // go to the start of the name data
            stream.BaseStream.Position = startOffset + alloc.Offset;

            // read the first header to get the length of the first name
            var nameHeader = new NameHeader(stream.ReadByte());

            // 0 as the length marks the end of the names in the folder
            var lst = new List<FileOrFolderName>();
            while (nameHeader.Length.ToInt32() != 0)
            {
                var name = new FileOrFolderName()
                {
                    IsFolder = nameHeader.IsFolder,
                    Name = stream.ReadBytes(nameHeader.Length.ToInt32())
                };

                if (nameHeader.IsFolder)
                {
                    name.ContentsIndexIfFolder = new FolderIndex(stream.ReadUInt16());
                }

                lst.Add(name);

                nameHeader = new NameHeader(stream.ReadByte());
            }

            return lst;
        }
    
        static bool ArraysAreEqual(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
            {
                return false;
            }

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static uint GetFatEntryIndex(BinaryReader stream, long startOffset, string filePath)
        {
            // just to allow users to use the intuitive leading slash
            if (filePath.StartsWith("/"))
            {
                filePath = filePath.Substring(1);
            }

            // Get each segment of the path, casting it to the required byte array
            byte[][] pathSegments = filePath.Split('/')
                                            .Select(Encoding.UTF8.GetBytes)
                                            .ToArray();

            // Initialise contents with the root folder contents
            // keep the allocation as a variable to allow the FAT offset to be calculated when the file is reached
            var alloc = GetRootFolderAllocationData(stream, startOffset);
            var contents = GetContents(stream, startOffset, alloc);

            // iterate through the segments. for loop used just so you know if it's the final item
            for (int i = 0; i < pathSegments.Length; i++)
            {
                var seg = pathSegments[i];
                // compare the path segment (i.e. file/folder name) to each of the names in the current folder
                // keeping track of the current index in the contents to allow the fat offset to be calculated
                bool found = false;
                for (int indexWithinFolder = 0; indexWithinFolder < contents.Count; indexWithinFolder++)
                {
                    var cont = contents[indexWithinFolder];

                    // if the names are equal, then proceed
                    if (ArraysAreEqual(cont.Name, seg))
                    {
                        if (i == pathSegments.Length - 1) // is the final segment in the path, i.e the file itself
                        {
                            return alloc.FatTopFileId + (uint)indexWithinFolder;
                        }
                        // else
                        alloc = GetAllocationData(stream, startOffset, cont.ContentsIndexIfFolder);
                        contents = GetContents(stream, startOffset, alloc);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new FileNotFoundException($"{nameof(GetFatEntryIndex)}: requested file ({filePath}) not found in {nameof(NdsNameTable)}");
                }
            }

            throw new Exception($"Arrived at unexpected point in {nameof(GetFatEntryIndex)}");
        }

        public static uint GetFatEntryIndex(BinaryReader stream, string filePath)
        {
            return GetFatEntryIndex(stream, GetStartOffset(stream), filePath);
        }
    }
}
