using System.IO;

namespace RanseiLink.Core.Graphics;


public class NCGR
{
    public const string MagicNumber = "RGCN";
    public static readonly string[] FileExtensions = new[] { ".ncgr", ".rgcn" };

    public CHAR Pixels { get; set; }

    public static NCGR Load(string file)
    {
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            return new NCGR(br);
        }
    }

    public void Save(string file)
    {
        using (var bw = new BinaryWriter(File.Create(file)))
        {
            WriteTo(bw);
        }
    }

    public NCGR(BinaryReader br)
    {
        long initOffset = br.BaseStream.Position;

        // first a typical file header
        var header = new NitroFileHeader(br);

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }

        // read 
        Pixels = new CHAR(br);

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public void WriteTo(BinaryWriter bw)
    {
        long initOffset = bw.BaseStream.Position;

        var header = new NitroFileHeader
        {
            MagicNumber = MagicNumber,
            ByteOrderMarker = 0xFEFF,
            Version = 0x0101,
            ChunkCount = 1,
            HeaderLength = 0x10
        };

        bw.Pad(header.HeaderLength);

        Pixels.WriteTo(bw);

        var endOffset = bw.BaseStream.Position;
        header.FileLength = (uint)(endOffset - initOffset);
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
    }


    public class CHAR
    {
        public struct Header
        {
            public const string MagicNumber = "RAHC";
            public uint TotalLength;
            public short TilesPerColumn;
            public short TilesPerRow;
            public TexFormat Format;
            public ushort Unknown1;
            public ushort Unknown2;
            public bool IsTiled;
            public int DataLength;
            public const int DataStartOffset = 0x18;

            public Header(BinaryReader br)
            {
                var magicNumber = br.ReadMagicNumber();
                if (magicNumber != MagicNumber)
                {
                    throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                }

                TotalLength = br.ReadUInt32();
                TilesPerColumn = br.ReadInt16();
                TilesPerRow = br.ReadInt16();
                Format = (TexFormat)br.ReadUInt32();
                Unknown1 = br.ReadUInt16();
                Unknown2 = br.ReadUInt16();
                IsTiled = br.ReadInt32() == 0;
                DataLength = br.ReadInt32();
                var dataStartOffset = br.ReadInt32();
                if (dataStartOffset != DataStartOffset)
                {
                    throw new InvalidDataException($"Unexcepted data start offset '{dataStartOffset} (expected: {dataStartOffset}");
                }
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.WriteMagicNumber(MagicNumber);
                bw.Write(TotalLength);
                bw.Write(TilesPerColumn);
                bw.Write(TilesPerRow);
                bw.Write((uint)Format);
                bw.Write(Unknown1);
                bw.Write(Unknown2);
                bw.Write(IsTiled ? 0 : 1);
                bw.Write(DataLength);
                bw.Write(DataStartOffset);
            }
        }

        public TexFormat Format { get; set; }
        public short TilesPerRow { get; set; }
        public short TilesPerColumn { get; set; }

        public ushort Unknown1 { get; set; }
        public ushort Unknown2 { get; set; }
        public bool IsTiled { get; set; }

        public byte[] Data { get; set; }

        public CHAR(BinaryReader br)
        {
            var initOffset = br.BaseStream.Position;
            var header = new Header(br);
            TilesPerColumn = header.TilesPerColumn;
            TilesPerRow = header.TilesPerRow;
            Unknown1 = header.Unknown1;
            Unknown2 = header.Unknown2;
            IsTiled = header.IsTiled;
            Format = header.Format;
            Data = br.ReadBytes(header.DataLength);
            if (header.Format == TexFormat.Pltt16)
            {
                Data = RawChar.Decompress(Data);
            }
            br.BaseStream.Position = initOffset + header.TotalLength;
        }

        public void WriteTo(BinaryWriter bw)
        {
            var startOffset = bw.BaseStream.Position;

            var header = new Header
            {
                TilesPerRow = TilesPerRow,
                TilesPerColumn = TilesPerColumn,
                Format = Format,
                Unknown1 = Unknown1,
                Unknown2 = Unknown2,
                IsTiled = IsTiled,
            };

            // skip header until later
            bw.Pad(Header.DataStartOffset + 8); // the data start offset is relative to the end of the generic chunk header which is magic num and total length i.e. 8

            // write data
            var data = Data;
            if (Format == TexFormat.Pltt16)
            {
                data = RawChar.Compress(data);
            }
            bw.Write(data);

            header.DataLength = data.Length;

            var endOffset = bw.BaseStream.Position;

            header.TotalLength = (uint)(endOffset - startOffset);

            // write header
            bw.BaseStream.Position = startOffset;
            header.WriteTo(bw);
            bw.BaseStream.Position = endOffset;
        }
    }
}