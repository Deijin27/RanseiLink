using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.Core.Models;

public class EVE
{
    public struct Header
    {
        public const string MagicNumber = "LSP\x0";
        public uint FortyEight; // 0x48
        public uint FileId; // file name is this in hex
        public uint NumStartSectionItems; // 0x1B
        public uint NumOffsets;
        public uint TableBDataOffset; // last entry in offset table A
        public uint Unknown;
        public uint FileLength;

        public Header(BinaryReader br)
        {
            var magicNumber = br.ReadMagicNumber();
            if (magicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
            }
            FortyEight = br.ReadUInt32();
            FileId = br.ReadUInt32();
            NumStartSectionItems = br.ReadUInt32();
            NumOffsets = br.ReadUInt32();
            TableBDataOffset = br.ReadUInt32();
            Unknown = br.ReadUInt32();
            FileLength = br.ReadUInt32();
        }
    }

    private const int OffsetTableStart = 0x100;

    public EVE(string file)
    {
        Console.WriteLine(file);
        using var br = new BinaryReader(File.OpenRead(file));

        var header = new Header(br);

        Console.WriteLine("StartSectionUnknowns:");
        uint[] unknowns = new uint[header.NumStartSectionItems];
        for (int i = 0; i < header.NumStartSectionItems; i++)
        {
            unknowns[i] = br.ReadUInt32();
            Console.WriteLine($"0x{unknowns[i]:X}");
        }

        Console.WriteLine("OffsetTableA:");
        br.BaseStream.Position = OffsetTableStart;
        uint[] offsetsA = new uint[header.NumOffsets];
        for (int i = 0; i < header.NumOffsets; i++)
        {
            offsetsA[i] = br.ReadUInt32();
            Console.WriteLine($"0x{offsetsA[i]:X}");
        }

        var shift = br.ReadUInt32();
        if (shift != header.TableBDataOffset)
        {
            throw new InvalidDataException("Listed offset doesnt match earlier value");
        }

        Console.WriteLine("OffsetTableB:");
        uint[] offsetsB = new uint[header.NumOffsets];
        for (int i = 0; i < header.NumOffsets; i++)
        {
            offsetsB[i] = br.ReadUInt32();
            Console.WriteLine($"0x{offsetsB[i]:X}");
        }

        var end = br.ReadUInt32();

        byte[] unknownBytes = br.ReadBytes(0x40);

        var itemStartOffset = br.BaseStream.Position;

        


    }
}
