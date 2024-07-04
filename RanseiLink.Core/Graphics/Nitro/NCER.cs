using System.Diagnostics;

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
        try
        {
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                return new NCER(br);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error loading file '{file}'", e);
        }
    }

    public void Save(string file)
    {
        using (var bw = new BinaryWriter(File.Create(file)))
        {
            WriteTo(bw);
        }
    }

    public NCER()
    {
        Clusters = new();
        Labels = new();
        Unknown = new();
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
        Clusters = new CEBK(br);
        Labels = new LABL(br);
        Unknown = new UEXT(br);

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public ushort Version { get; set; }
    public CEBK Clusters { get; set; }
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

        Clusters.WriteTo(bw);
        Labels.WriteTo(bw);
        Unknown.WriteTo(bw);

        var endOffset = bw.BaseStream.Position;
        header.FileLength = (uint)(endOffset - initOffset);
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
        bw.BaseStream.Position = endOffset;
    }
}

/// <summary>
/// Cell Bank (Which I've nicknamed "Cluster")
/// </summary>
public class CEBK
{
    public const string MagicNumber = "KBEC";

    public byte BlockSize { get; set; }
    public ushort BankType { get; set; }
    public List<Cluster> Clusters { get; set; }
    public UCAT? Ucat { get; set; }

    public struct SubHeader
    {
        public const int Length = 24;
        
        public ushort NumberOfBanks;
        public ushort BankType;
        public uint BankDataOffset;
        public byte BlockSize;
        public uint PartitionDataOffset; // default 0 if none?
        public uint Unknown;
        public uint UcatDataOffset;

        public SubHeader(BinaryReader br)
        {
            NumberOfBanks = br.ReadUInt16();
            BankType = br.ReadUInt16();
            BankDataOffset = br.ReadUInt32();
            BlockSize = br.ReadByte();
            br.Skip(3);
            PartitionDataOffset = br.ReadUInt32();
            Unknown = br.ReadUInt32();
            UcatDataOffset = br.ReadUInt32();
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(NumberOfBanks);
            bw.Write(BankType);
            bw.Write(BankDataOffset);
            bw.Write(BlockSize);
            bw.Pad(3);
            bw.Write(PartitionDataOffset);
            bw.Write(Unknown); 
            bw.Write(UcatDataOffset);
        }
    }

    public CEBK()
    {
        Clusters = new();
    }

    public CEBK(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var nitroHeader = new NitroChunkHeader(br);
        if (nitroHeader.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{nitroHeader.MagicNumber}'. (expected: {MagicNumber})");
        }
        var postHeaderOffset = br.BaseStream.Position;
        var header = new SubHeader(br);
        BlockSize = header.BlockSize;
        BankType = header.BankType;
        if (header.PartitionDataOffset != 0)
        {
            throw new NotImplementedException("Partition data in NCER not supported");
        }

        // Read Bank Data

        br.BaseStream.Position = postHeaderOffset + header.BankDataOffset;
        var bankInfos = new BankInfo[header.NumberOfBanks];
        for (int i = 0; i < header.NumberOfBanks; i++)
        {
            bankInfos[i] = new BankInfo(br, header.BankType);
        }
        Clusters = new List<Cluster>(header.NumberOfBanks);
        for (int i = 0; i < header.NumberOfBanks; i++)
        {
            var bankInfo = bankInfos[i];
            var bank = new Cluster(bankInfo.NumberOfCells);
            for (ushort j = 0; j < bankInfo.NumberOfCells; j++)
            {
                var cell = new Cell(br);

                bank.Add(cell);
            }
            bank.ReadOnlyCellInfo = bankInfo.ReadOnlyCellInfo;
            bank.XMax = bankInfo.XMax;
            bank.YMax = bankInfo.YMax;
            bank.XMin = bankInfo.XMin; 
            bank.YMin = bankInfo.YMin;
            Clusters.Add(bank);
        }

        if (header.UcatDataOffset != 0)
        {
            br.BaseStream.Position = postHeaderOffset + header.UcatDataOffset;
            Ucat = new UCAT(br);
        }


        // Go to end

        br.BaseStream.Position = initOffset + nitroHeader.ChunkLength;
    }

    internal void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        var postHeaderOffset = initOffset + NitroChunkHeader.Length;

        var nitroHeader = new NitroChunkHeader
        {
            MagicNumber = MagicNumber
        };
        var header = new SubHeader
        {
            NumberOfBanks = (ushort)Clusters.Count,
            BankType = BankType,
            BlockSize = BlockSize,
            PartitionDataOffset = 0,
            BankDataOffset = SubHeader.Length
        };

        bw.Pad(NitroChunkHeader.Length + SubHeader.Length + Clusters.Count * BankInfo.Length(BankType));


        // write banks
        var cellStart = bw.BaseStream.Position;
        var bankInfo = new BankInfo[Clusters.Count];
        for (var i = 0; i < Clusters.Count; i++)
        {
            var bank = Clusters[i];
            var info = new BankInfo
            {
                NumberOfCells = (ushort)bank.Count,
                CellOffset = (uint)(bw.BaseStream.Position - cellStart),
                ReadOnlyCellInfo = bank.ReadOnlyCellInfo
            };
            if (BankType == 1)
            {
                info.XMax = (short)bank.XMax;
                info.YMax = (short)bank.YMax;
                info.XMin = (short)bank.XMin;
                info.YMin = (short)bank.YMin;
            }
            
            foreach (var cell in bank)
            {
                cell.WriteTo(bw);
            }

            bankInfo[i] = info;
        }

        if (Ucat != null)
        {
            header.UcatDataOffset = (uint)(bw.BaseStream.Position - postHeaderOffset);
            Ucat.WriteTo(bw, BankType);
        }

        // write header
        var endOffset = bw.BaseStream.Position;
        nitroHeader.ChunkLength = (uint)(endOffset - initOffset);
        while (nitroHeader.ChunkLength % 4 != 0)
        {
            bw.Write((byte)0);
            nitroHeader.ChunkLength++;
            endOffset++;
        }
        bw.BaseStream.Position = initOffset;
        nitroHeader.WriteTo(bw);
        header.WriteTo(bw);
        foreach (var info in bankInfo)
        {
            info.WriteTo(bw, BankType);
        }
        bw.BaseStream.Position = endOffset;
    }

    public class UCAT
    {
        public const string MagicNumber = "TACU";

        public List<uint> SomeStuff { get; set; } = new List<uint>();
        public List<uint> SomeZeros { get; set; } = new List<uint>();

        public UCAT(BinaryReader br)
        {
            var init = br.BaseStream.Position;
            var header = new NitroChunkHeader(br); // this isn't a chunk, but it uses the same simple header format, followed by the subheader
            if (header.MagicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number '{header.MagicNumber}'. (expected: {MagicNumber})");
            }
            var postHeaderOffset = br.BaseStream.Position;

            // these two have the same values as the bank in my file, so I'm assuming it's the same stuff again
            ushort bankCount = br.ReadUInt16();
            ushort bankType = br.ReadUInt16();
            uint dataOffset = br.ReadUInt32();

            br.BaseStream.Position = postHeaderOffset + dataOffset;

            // theres a set of uints of length bankCount
            for (int i = 0; i < bankCount; i++)
            {
                SomeStuff.Add(br.ReadUInt32());
            }
            // then some set of zeros in my file
            for (int i = 0; i < bankCount; i++)
            {
                SomeZeros.Add(br.ReadUInt32());
            }

            br.BaseStream.Position = init + header.ChunkLength;
        }

        public void WriteTo(BinaryWriter bw, int bankType)
        {
            var init = bw.BaseStream.Position;
            var header = new NitroChunkHeader
            {
                MagicNumber = MagicNumber 
            };
            bw.Pad(NitroChunkHeader.Length);
            bw.Write((ushort)SomeStuff.Count);
            bw.Write((ushort)bankType);
            bw.Write((uint)8); // offset of stuff relative to end of nitro header

            foreach (var stuff in SomeStuff)
            {
                bw.Write(stuff);
            }

            foreach (var zero in SomeZeros)
            {
                bw.Write(zero);
            }

            var end = bw.BaseStream.Position;
            header.ChunkLength = (uint)(end - init);
            bw.BaseStream.Position = init;
            header.WriteTo(bw);
            bw.BaseStream.Position = end;
        }
    }
}

public struct BankInfo
{
    public static int Length(ushort bankType)
    {
        int length = 8;
        if (bankType == 1)
        {
            length += 8;
        }
        return length;
    }

    public ushort NumberOfCells;
    public ushort ReadOnlyCellInfo;
    public uint CellOffset;

    public short XMax;
    public short YMax;
    public short XMin;
    public short YMin;

    public BankInfo(BinaryReader br, ushort bankType)
    {
        NumberOfCells = br.ReadUInt16();
        ReadOnlyCellInfo = br.ReadUInt16();
        CellOffset = br.ReadUInt32();
        if (bankType == 1)
        {
            XMax = br.ReadInt16();
            YMax = br.ReadInt16();
            XMin = br.ReadInt16();
            YMin = br.ReadInt16();
        }
        else
        {
            XMax = 0;
            YMax = 0;
            XMin = 0;
            YMin = 0;
        }
    }

    public void WriteTo(BinaryWriter bw, ushort bankType)
    {
        bw.Write(NumberOfCells);
        bw.Write(ReadOnlyCellInfo);
        bw.Write(CellOffset);
        if (bankType == 1)
        {
            bw.Write(XMax);
            bw.Write(YMax); 
            bw.Write(XMin);
            bw.Write(YMin);
        }
    }
}

public class Cluster : List<Cell>
{
    public Cluster() : base() { }
    public Cluster(int capacity) : base(capacity) { }
    public Cluster(IEnumerable<Cell> collection) : base(collection) { }


    public ushort ReadOnlyCellInfo { get; set; }

    // these are the min/max non-transparent pixel across all cells in this bank
    // I assume they're used for optimising draw times? although I don't see it giving much improvement over just using the cell sizes and positions for this
    // we just use the bank positions and sizes to estimate this, hopefully that wouldn't break anything?
    // otherwise we will have to calculate this while the cells are still images
    public int XMax { get; set; }
    public int YMax { get; set; }
    public int XMin { get; set; }
    public int YMin { get; set; }

    public void EstimateMinMaxValues()
    {
        XMax = 0;
        YMax = 0;
        if (Count == 0)
        {
            XMin = 0;
            YMin = 0;
            return;
        }
        XMin = int.MaxValue;
        YMin = int.MaxValue;
        foreach (var cell in this)
        {
            // we minus 1 because we're working with pixels
            // if xmin is 2, and width is 1, then xmax is also 2, ya see?
            var cellXMax = cell.XOffset + cell.Width - 1; 
            var cellYMax = cell.YOffset + cell.Height - 1;
            if (cellXMax > XMax)
            {
                XMax = cellXMax;
            }
            if (cellYMax > YMax)
            {
                YMax = cellYMax;
            }
            if (cell.XOffset < XMin)
            {
                XMin = cell.XOffset;
            }
            if (cell.YOffset < YMin)
            {
                YMin = cell.YOffset;
            }
        }
    }
}

[DebuggerDisplay("Cell(TileOff={TileOffset} X={XOffset}, Y={YOffset}, W={Width}, H={Height})")]
public class Cell
{
    public override string ToString()
    {
        return $"Cell(TileOff={TileOffset} X={XOffset}, Y={YOffset}, W={Width}, H={Height})";
    }
    public int Width { get; set; }
    public int Height { get; set; }
    public ushort CellId { get; set; }

    public int YOffset { get; set; }
    public RotateOrScale RotateOrScale { get; set; }
    public bool ObjDisable { get; set; }
    public bool DoubleSize { get; set; }
    public ObjMode ObjMode { get; set; }
    public bool Mosaic { get; set; }
    public BitDepth Depth { get; set; }
    public Shape Shape { get; set; }

    public int XOffset { get; set; }
    public byte Unused { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public byte SelectParam { get; set; }
    public Scale Scale { get; set; }

    public int TileOffset { get; set; }
    public byte Priority { get; set; }
    public byte IndexPalette { get; set; }

    public Cell()
    {
        Width = 0;
        Height = 0;

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
        ObjMode = (ObjMode)((value0 >> 10) & 2); // what is thsi again, aninnesnitesnties
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
        Priority = (byte)((value2 >> 10) & 2); // what is this? why?
        IndexPalette = (byte)(value2 >> 12);

        // other

        var size = CellImageUtil.GetCellSize(Shape, Scale);
        Width = size.Width;
        Height = size.Height;

    }

    public void WriteTo(BinaryWriter bw)
    {
        // value 0

        int value0 = 0;

        value0 |= YOffset & 0xFF;
        value0 |= ((int)RotateOrScale & 1) << 8;
        bool nextFlag = RotateOrScale == RotateOrScale.Rotate ? ObjDisable : DoubleSize;
        value0 |= (nextFlag ? 1 : 0) << 9;
        value0 |= ((int)ObjMode & 2) << 10;
        value0 |= (Mosaic ? 1 : 0) << 12;
        value0 |= ((int)Depth & 1) << 13;
        value0 |= (int)Shape << 14;

        bw.Write((ushort)value0);

        // value1

        int value1 = 0;

        var storedXOffset = XOffset < 0 ? XOffset + 0x200 : XOffset;
        value1 |= storedXOffset & 0x1ff;
        if (RotateOrScale == RotateOrScale.Rotate)
        {
            value1 |= (Unused & 3) << 9;
            value1 |= (FlipX ? 1 : 0) << 12;
            value1 |= (FlipY ? 1 : 0) << 13;
        }
        else
        {
            value1 |= (SelectParam & 0x1f) << 9;
        }
        value1 |= (int)Scale << 14;

        bw.Write((ushort)value1);
        
        // value 2

        int value2 = 0;

        value2 |= TileOffset & 0x3ff;
        value2 |= (Priority & 2) << 10;
        value2 |= IndexPalette << 12;

        bw.Write((ushort)value2);
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
