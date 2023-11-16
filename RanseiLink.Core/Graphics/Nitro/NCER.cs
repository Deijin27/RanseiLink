using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Graphics;


/// <summary>
/// Cell Resource
/// </summary>
public class NCER
{
    public const string MagicNumber = "RECN";
    public static readonly string[] FileExtensions = new[] { ".necr", ".recn" };

    public static NCER Load(string file)
    {
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            return new NCER(br);
        }
    }

    public NCER(BinaryReader br)
    {
        long initOffset = br.BaseStream.Position;

        // first a typical file header
        var header = new NitroFileHeader(br);
        Version = header.Version;

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }

        if (header.ChunkCount != 3)
        {
            throw new Exception("Unexpected chunk count of NCER");
        }

        // read 
        CellBanks = new CEBK(br);
        Labels = new LABL(br);
        Unknown = new UEXT(br);

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public ushort Version { get; set; }
    public CEBK CellBanks { get; set; }
    public LABL Labels { get; set; }
    public UEXT Unknown { get; set; }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        var header = new NitroFileHeader
        {
            MagicNumber = MagicNumber,
            ByteOrderMarker = 0xFEFF,
            Version = Version,
            ChunkCount = 3,
            HeaderLength = 0x10
        };

        bw.Pad(header.HeaderLength);

        CellBanks.WriteTo(bw);
        Labels.WriteTo(bw);
        Unknown.WriteTo(bw);

        var endOffset = bw.BaseStream.Position;
        header.FileLength = (uint)(endOffset - initOffset);
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
    }
}

/// <summary>
/// Cell Bank
/// </summary>
public class CEBK
{
    public byte BlockSize { get; set; }
    public List<Cell[]> Banks { get; set; }

    public struct Header
    {
        public const string MagicNumber = "KBEC";
        public uint TotalLength;
        public ushort NumberOfBanks;
        public ushort BankType;
        public uint BankDataOffset;
        public byte BlockSize;
        public uint PartitionDataOffset;

        public Header(BinaryReader br)
        {
            var magicNumber = br.ReadMagicNumber();
            if (magicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
            }

            TotalLength = br.ReadUInt32();
            NumberOfBanks = br.ReadUInt16(); // 1
            BankType = br.ReadUInt16(); // 1
            BankDataOffset = br.ReadUInt32(); // 0x18
            BlockSize = br.ReadByte(); // 2
            br.Skip(3);
            PartitionDataOffset = br.ReadUInt32(); // 0
            br.Skip(8);
        }
    }

    public CEBK(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var header = new Header(br);
        BlockSize = header.BlockSize;
        if (header.PartitionDataOffset != 0)
        {
            throw new NotImplementedException("Partition data in NCER not supported");
        }

        br.BaseStream.Position = initOffset + header.BankDataOffset + 8;
        var bankInfos = new BankInfo[header.NumberOfBanks];
        for (int i = 0; i < header.NumberOfBanks; i++)
        {
            var bank = new BankInfo
            {
                NumberOfCells = br.ReadUInt16(),
                ReadOnlyCellInfo = br.ReadUInt16(),
                CellOffset = br.ReadUInt32()
            };
            if (header.BankType == 1)
            {
                bank.XMax = br.ReadUInt16();
                bank.YMax = br.ReadUInt16();
                bank.XMin = br.ReadUInt16();
                bank.YMin = br.ReadUInt16();
            }
            bankInfos[i] = bank;
        }
        Banks = new List<Cell[]>(header.NumberOfBanks);
        for (int i = 0; i < header.NumberOfBanks; i++)
        {
            var bankInfo = bankInfos[i];
            var bank = new Cell[bankInfo.NumberOfCells];

            for (int j = 0; j < bankInfo.NumberOfCells; j++)
            {
                var cell = new Cell(br)
                {
                    CellId = (ushort)j
                };

                // Calculate the size
                bank[j] = cell;
            }

            bank = bank.OrderBy(x => x.Priority).ThenBy(x => x.CellId).ToArray();

            Banks.Add(bank);
        }

        br.BaseStream.Position = initOffset + header.TotalLength;
    }

    internal void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;

        var header = new Header
        {
        };

        throw new NotImplementedException();
    }
}

public struct BankInfo
{
    public ushort NumberOfCells;
    public ushort ReadOnlyCellInfo;
    public uint CellOffset;

    public ushort XMax;
    public ushort YMax;
    public ushort XMin;
    public ushort YMin;
}


[DebuggerDisplay("Cell: TileOff={TileOffset} X={XOffset}, Y={YOffset}, W={Width}, H={Height}")]
public struct Cell
{
    public int Width;
    public int Height;
    public ushort CellId;

    public int YOffset;
    public RotateOrScale RotateOrScale;
    public bool ObjDisable;
    public bool DoubleSize;
    public ObjMode ObjMode;
    public bool Mosaic;
    public BitDepth Depth;
    public Shape Shape;

    public int XOffset;
    public byte Unused;
    public bool FlipX;
    public bool FlipY;
    public byte SelectParam;
    public Scale Scale;

    public int TileOffset;
    public byte Priority;
    public byte IndexPalette;

    public Cell()
    {
        Width = 0;
        Height = 0;
        CellId = 0;

        YOffset = 0;
        RotateOrScale = RotateOrScale.Rotate;
        ObjDisable = false;
        DoubleSize = false;
        ObjMode = ObjMode.Normal;
        Mosaic = false;
        Depth = BitDepth.e8Bit; // this is the only non-zero value that seems to be a default
        Shape = Shape.Square;

        XOffset = 0;
        Unused = 0;
        FlipX = false;
        FlipY = false;
        SelectParam = 0;
        Scale = Scale.Small;

        TileOffset = 0;
        Priority = 0;
        IndexPalette = 0;
    }

    public Cell(BinaryReader br)
    {
        // first ushort

        int value0 = br.ReadUInt16();

        YOffset = (sbyte)(value0 & 0xFF);
        RotateOrScale = (RotateOrScale)((value0 >> 8) & 1);
        bool nextFlag = ((value0 >> 9) & 1) == 1;
        if (RotateOrScale == RotateOrScale.Scale)
        {
            DoubleSize = nextFlag;
            ObjDisable = false;
        }
        else
        {
            DoubleSize = false;
            ObjDisable = nextFlag;
        }
        ObjMode = (ObjMode)((value0 >> 10) & 2);
        Mosaic = ((value0 >> 12) & 1) == 1;
        Depth = (BitDepth)((value0 >> 13) & 1);
        Shape = (Shape)(value0 >> 14);

        // second ushort

        int value1 = br.ReadUInt16();

        XOffset = value1 & 0x1ff;
        if (XOffset >= 0x100)
        {
            XOffset -= 0x200;
        }

        if (RotateOrScale == RotateOrScale.Rotate)
        {
            Unused = (byte)((value1 >> 9) & 3);
            FlipX = ((value1 >> 12) & 1) == 1;
            FlipY = ((value1 >> 13) & 1) == 1;
            SelectParam = 0;
        }
        else
        {
            Unused = 0;
            FlipX = false;
            FlipY = false;
            SelectParam = (byte)((value1 >> 9) & 0x1f);
        }
        Scale = (Scale)(value1 >> 14);

        // third ushort

        int value2 = br.ReadUInt16();

        TileOffset = value2 & 0x3ff;
        Priority = (byte)((value2 >> 10) & 2);
        IndexPalette = (byte)(value2 >> 12);

        // other

        var size = CellImageUtil.GetCellSize(Shape, Scale);
        Width = size.Width;
        Height = size.Height;
        CellId = 0;

    }
}


public enum ObjMode
{
    Normal,
    SemiTransparent,
    Window
}

public enum Shape
{
    Square,
    Wide,
    Tall,
}

public enum Scale
{
    Small,
    Medium,
    Large,
    XLarge,
}

public enum BitDepth
{
    e4Bit,
    e8Bit,
}

public enum RotateOrScale
{
    Rotate,
    Scale,
}



/// <summary>
/// Cell Labels
/// </summary>
public class LABL
{
    public List<string> Names { get; set; } = new List<string>();

    public const string MagicNumber = "LBAL";

    public LABL(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var header = new NitroChunkHeader(br);
        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{header.MagicNumber}' at offset 0x{initOffset:X}. (expected: {MagicNumber})");
        }
        var endOffset = initOffset + header.ChunkLength;

        // it's weird that they don't store the number of names
        // it doesn't always correspond to the number of banks
        // this is the bests I could come up with to consistently work
        var labelOffsets = new List<uint>();
        uint offset = br.ReadUInt32();
        while (offset <= 0xFFFF)
        {
            labelOffsets.Add(offset);
            offset = br.ReadUInt32();
        }

        var nameStart = br.BaseStream.Position - 4;

        var buffer = new byte[50];
        // The labels are null-terminated strings
        foreach (var lblOff in labelOffsets)
        {
            br.BaseStream.Position = nameStart + lblOff;
            Names.Add(br.ReadNullTerminatedString(buffer));
        }

        br.BaseStream.Position = endOffset;
    }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        var header = new NitroChunkHeader
        {
            MagicNumber = MagicNumber,
        };

        bw.Pad(NitroChunkHeader.Length + Names.Count * 4);
        uint[] nameOffsets = new uint[Names.Count];
        var nameStart = bw.BaseStream.Position;
        for (int i = 0; i < nameOffsets.Length; i++)
        {
            nameOffsets[i] = (uint)(bw.BaseStream.Position - nameStart);
            bw.WriteNullTerminatedString(Names[i]);
        }

        var endOffset = bw.BaseStream.Position;
        header.ChunkLength = (uint)(endOffset - initOffset);

        // write header
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
        foreach (uint name in nameOffsets)
        {
            bw.Write(name);
        }

        bw.BaseStream.Position = endOffset;
    }
}

/// <summary>
/// Whomst are youmst?
/// </summary>
public class UEXT
{
    public uint Unknown { get; set; }

    public const string MagicNumber = "TXEU";

    public UEXT(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var header = new NitroChunkHeader(br);
        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{header.MagicNumber}' at offset 0x{initOffset:X}. (expected: {MagicNumber})");
        }

        Unknown = br.ReadUInt32();

        br.BaseStream.Position = initOffset + header.ChunkLength;
    }

    public void WriteTo(BinaryWriter bw)
    {
        var header = new NitroChunkHeader
        {
            MagicNumber = MagicNumber,
            ChunkLength = NitroChunkHeader.Length + 4
        };
        header.WriteTo(bw);
        bw.Write(Unknown);
    }
} 