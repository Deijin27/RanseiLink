#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanseiLink.Core.Graphics;

public struct RawRadixNode
{
    public const int Length = 4;
    
    public byte RefBit;
    public byte IdxLeft;
    public byte IdxRight;
    public byte IdxEntry;

    public RawRadixNode(byte refBit, byte idxLeft, byte idxRight, byte idxEntry)
    {
        RefBit = refBit;
        IdxLeft = idxLeft;
        IdxRight = idxRight;
        IdxEntry = idxEntry;
    }

    public RawRadixNode(BinaryReader br)
    {
        RefBit = br.ReadByte();
        IdxLeft = br.ReadByte();
        IdxRight = br.ReadByte();
        IdxEntry = br.ReadByte();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(RefBit);
        bw.Write(IdxLeft);
        bw.Write(IdxRight);
        bw.Write(IdxEntry);
    }
}

public interface IRadixData
{
    ushort Length { get; }
    void ReadFrom(BinaryReader br);
    void WriteTo(BinaryWriter bw);
}

/// <summary>
/// Common type of radix data
/// </summary>
public class OffsetRadixData : IRadixData
{
    public ushort Length => 4;

    public uint Offset { get; set; }

    public void ReadFrom(BinaryReader br)
    {
        Offset = br.ReadUInt32();
    }

    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(Offset);
    }
}

public class RadixDict<TRadixData> where TRadixData : IRadixData, new()
{
    public struct HeaderA
    {
        public const int Length = 8;
        public byte Revision;
        public byte EntryCount;
        public ushort TotalSize;
        public ushort RadixTreeOffset;
        public ushort HeaderBOffset;

        public HeaderA(BinaryReader br)
        {
            Revision = br.ReadByte();
            EntryCount = br.ReadByte();
            TotalSize = br.ReadUInt16();
            RadixTreeOffset = br.ReadUInt16();
            HeaderBOffset = br.ReadUInt16();
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(Revision);
            bw.Write(EntryCount);
            bw.Write(TotalSize);
            bw.Write(RadixTreeOffset);
            bw.Write(HeaderBOffset);
        }
    }

    public struct HeaderB
    {
        public const int Length = 4;

        public ushort RadixDataEntrySize;
        public ushort NamesOffset;

        public HeaderB(BinaryReader br)
        {
            RadixDataEntrySize = br.ReadUInt16();
            NamesOffset = br.ReadUInt16();
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(RadixDataEntrySize);
            bw.Write(NamesOffset);
        }
    }


    public List<TRadixData> Data { get; set; } = new List<TRadixData>();
    public List<string> Names { get; set; } = new List<string>();

    public RadixDict()
    {
    }

    public static int CalculateLength(int numEntries)
    {
        return HeaderA.Length
            + RawRadixNode.Length * (numEntries + 1)
            + HeaderB.Length
            + new TRadixData().Length * numEntries
            + NameLength * numEntries;

    }

    public RadixDict(BinaryReader br)
    {
        // read header A
        var headerA = new HeaderA(br);
        if (headerA.Revision != 0)
        {
            throw new Exception($"Unexpeced revision value {headerA.Revision} in radix dict. Please report the file this occurred with");
        }

        // read radix tree
        var RadixTree = new List<RawRadixNode>();
        for (byte i = 0; i < headerA.EntryCount + 1; i++) // each name, plus the root node
        {
            RadixTree.Add(new RawRadixNode(br));
        }

        // read header B
        var headerB = new HeaderB(br);
        if (headerB.RadixDataEntrySize != new TRadixData().Length)
        {
            throw new Exception("Unexpected radix data entry size. Please report the file this occurred with");
        }

        // read data
        for (int i = 0; i < headerA.EntryCount; i++)
        {
            var data = new TRadixData();
            data.ReadFrom(br);
            Data.Add(data);
        }

        // read names
        for (int i = 0; i < headerA.EntryCount; i++)
        {
            Names.Add(ReadName(br));
        }
    }

    public void WriteTo(BinaryWriter bw)
    {
        // create headers
        var headerA = new HeaderA
        {
            Revision = 0,
            EntryCount = (byte)Names.Count,
            RadixTreeOffset = HeaderA.Length
        };
        var headerB = new HeaderB
        {
            RadixDataEntrySize = Data[0].Length
        };

        // skip headerA for now
        var initOffset = bw.BaseStream.Position;
        bw.Pad(HeaderA.Length);

        // write radix tree
        string[] names = Names.ToArray();
        var tree = RadixTreeGenerator.Generate(names);
        foreach (var node in tree)
        {
            node.WriteTo(bw);
        }

        // skip headerB for now
        var headerBOffset = bw.BaseStream.Position;
        headerA.HeaderBOffset = (ushort)(headerBOffset - initOffset);
        bw.Pad(HeaderB.Length);
        
        // write data entries
        foreach (var i in Data)
        {
            i.WriteTo(bw);
        }

        // write names
        headerB.NamesOffset = (ushort)(bw.BaseStream.Position - headerBOffset);
        foreach (var name in Names)
        {
            WriteName(bw, name);
        }

        // write headers
        var endOffset = bw.BaseStream.Position;
        headerA.TotalSize = (ushort)(endOffset - initOffset);

        bw.BaseStream.Position = initOffset;
        headerA.WriteTo(bw);

        bw.BaseStream.Position = headerBOffset;
        headerB.WriteTo(bw);

        bw.BaseStream.Position = endOffset;
    }

    public const int NameLength = 16;
    public static string ReadName(BinaryReader br)
    {
        return Encoding.UTF8.GetString(br.ReadBytes(NameLength)).TrimEnd('\x0');
    }

    public static void WriteName(BinaryWriter bw, string name)
    {
        var bytes = Encoding.UTF8.GetBytes(name);
        Array.Resize(ref bytes, NameLength);
        bw.Write(bytes);
    }
}