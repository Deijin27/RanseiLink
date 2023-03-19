using System;
using System.Collections.Generic;
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

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }


        // read 
        CellBanks = new CEBK(br);
        Labels = new LABL(br, CellBanks.Banks.Count);
        Unknown = new UEXT(br);

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public CEBK CellBanks { get; set; }
    public LABL Labels { get; set; }
    public UEXT Unknown { get; set; }
}

/// <summary>
/// Cell Bank
/// </summary>
public class CEBK
{
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

    public byte BlockSize { get; set; }

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



    public List<Cell[]> Banks { get; set; }

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


public struct Cell
{
    public int Width;
    public int Height;
    public ushort CellId;

    public int YOffset;
    public RotateScaleFlag RotateScaleFlag;
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

    public Cell(BinaryReader br)
    {
        // first ushort

        int value0 = br.ReadUInt16();

        YOffset = (sbyte)(value0 & 0xFF);
        RotateScaleFlag = (RotateScaleFlag)((value0 >> 8) & 1);
        bool nextFlag = ((value0 >> 9) & 1) == 1;
        if (RotateScaleFlag == RotateScaleFlag.Scale)
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

        if (RotateScaleFlag == RotateScaleFlag.Rotate)
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

        var size = CellSize(Shape, Scale);
        Width = size.Width;
        Height = size.Height;
        CellId = 0;

    }

    private static Size CellSize(Shape shape, Scale scale)
    {
        switch (shape)
        {
            case Shape.Square:
                switch (scale)
                {
                    case Scale.Small: return new Size(8, 8);
                    case Scale.Medium: return new Size(16, 16);
                    case Scale.Large: return new Size(32, 32);
                    case Scale.XLarge: return new Size(64, 64);
                    default: throw new ArgumentException($"Parameter {nameof(scale)} has invalid value {scale}");
                }
            case Shape.Wide: 
                switch (scale)
                {
                    case Scale.Small: return new Size(16, 8);
                    case Scale.Medium: return new Size(32, 8);
                    case Scale.Large: return new Size(32, 16);
                    case Scale.XLarge: return new Size(64, 32);
                    default: throw new ArgumentException($"Parameter {nameof(scale)} has invalid value {scale}");
                }
            case Shape.Tall: 
                switch (scale)
                {
                    case Scale.Small: return new Size(8, 16);
                    case Scale.Medium: return new Size(8, 32);
                    case Scale.Large: return new Size(16, 32);
                    case Scale.XLarge: return new Size(32, 64);
                    default: throw new ArgumentException($"Parameter {nameof(scale)} has invalid value {scale}");
                }
            default: throw new ArgumentException($"Parameter {nameof(shape)} has invalid value {shape}");
        };
    }

    struct Size
    {
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public int Width;
        public int Height;
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

public enum RotateScaleFlag
{
    Rotate,
    Scale,
}



/// <summary>
/// Cell Labels
/// </summary>
public class LABL
{
    public string[] Names;

    public const string MagicNumber = "LBAL";

    public LABL(BinaryReader br, int numberOfBanks)
    {
        var initOffset = br.BaseStream.Position;
        var magicNumber = br.ReadMagicNumber();
        if (magicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{magicNumber}' at offset 0x{initOffset:X}. (expected: {MagicNumber})");
        }
        uint totalLength = br.ReadUInt32();

        uint[] nameOffsets = new uint[numberOfBanks];
        for (int i = 0; i < numberOfBanks; i++)
        {
            nameOffsets[i] = br.ReadUInt32();
        }
        var nameStart = br.BaseStream.Position;
        Names = new string[numberOfBanks];
        for (int i = 0; i < numberOfBanks; i++)
        {
            br.BaseStream.Position = nameStart + nameOffsets[i];
            string name = "";
            byte b = br.ReadByte();
            while (b != 0)
            {
                name += (char)b;
                b = br.ReadByte();
            }
            Names[i] = name;
        }

        br.BaseStream.Position = initOffset + totalLength;
    }
}

/// <summary>
/// Whomst are youmst?
/// </summary>
public class UEXT
{
    public uint Unknown { get; }

    public const string MagicNumber = "TXEU";

    public UEXT(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var magicNumber = br.ReadMagicNumber();
        if (magicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{magicNumber}' at offset 0x{initOffset:X}. (expected: {MagicNumber})");
        }
        uint totalLength = br.ReadUInt32();

        Unknown = br.ReadUInt32();

        br.BaseStream.Position = initOffset + totalLength;
    }
} 