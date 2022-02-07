using System.IO;

namespace RanseiLink.Core.Graphics;

public  class NCGR
{
    public const string MagicNumber = "RGCN";
    public static readonly string[] FileExtensions = new[] { ".ncgr", ".rgcn" };

    public static NCGR Load(string file)
    {
        using var br = new BinaryReader(File.OpenRead(file));
        return new NCGR(br);
    }

    public NCGR(BinaryReader br)
    {
        long initOffset = br.BaseStream.Position;

        // first a typical file header
        var header = new GenericFileHeader(br);

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }

        // read 
        Pixels = new CHAR(br);

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public CHAR Pixels { get; set; }


    public class CHAR
    {
        public struct Header
        {
            public const string MagicNumber = "RAHC";
            public uint TotalLength;
            public ushort TilesPerColumn;
            public ushort TilesPerRow;
            public BitsPerPixel BitsPerPixel;
            public ushort Unknown1;
            public ushort Unknown2;
            public uint TiledFlag;
            public TileForm Order;
            public int DataLength;
            public uint Unknown3;

            public Header(BinaryReader br)
            {
                var magicNumber = br.ReadMagicNumber();
                if (magicNumber != MagicNumber)
                {
                    throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                }

                TotalLength = br.ReadUInt32();
                TilesPerColumn = br.ReadUInt16();
                TilesPerRow = br.ReadUInt16();
                BitsPerPixel = (BitsPerPixel)br.ReadUInt32();
                Unknown1 = br.ReadUInt16();
                Unknown2 = br.ReadUInt16();
                TiledFlag = br.ReadUInt32();
                if ((TiledFlag & 0xFF) == 0x0)
                    Order = TileForm.Horizontal;
                else
                    Order = TileForm.Lineal;
                DataLength = br.ReadInt32();
                Unknown3 = br.ReadUInt32();
            }
        }

        public int TilesPerColumn;

        public CHAR(BinaryReader br)
        {
            var initOffset = br.BaseStream.Position;
            var header = new Header(br);
            TilesPerColumn = header.TilesPerColumn;
            Data = br.ReadBytes(header.DataLength);
            if (header.BitsPerPixel == BitsPerPixel.FourBitsPerPixel)
            {
                Data = RawChar.Decompress(Data);
            }
            br.BaseStream.Position = initOffset + header.TotalLength;
        }

        public byte[] Data { get; set; }
    }
}

public enum BitsPerPixel
{
    FourBitsPerPixel = 3,
    EightBitsPerPixel = 4,
}

public enum TileForm
{
    Horizontal,
    Lineal,
}
