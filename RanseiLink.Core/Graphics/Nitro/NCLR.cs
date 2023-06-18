using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// Nitro Color Resource
/// </summary>
public class NCLR
{
    public const string MagicNumber = "RLCN";
    public static readonly string[] FileExtensions = new[] { ".nclr", ".rlcn" };

    public PLTT Palettes { get; set; }
    public PCMP? PaletteCollectionMap { get; set; }

    public static NCLR Load(string file)
    {
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            return new NCLR(br);
        } 
    }

    public void Save(string file)
    {
        using (var bw = new BinaryWriter(File.Create(file)))
        {
            WriteTo(bw);
        }
    }

    public NCLR(BinaryReader br)
    {
        long initOffset = br.BaseStream.Position;

        // first a typical file header
        var header = new NitroFileHeader(br);

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }

        // read 
        Palettes = new PLTT(br);

        if (header.ChunkCount == 2)
        {
            PaletteCollectionMap = new PCMP(br);
        }

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public void WriteTo(BinaryWriter bw)
    {
        long initOffset = bw.BaseStream.Position;

        var header = new NitroFileHeader
        {
            MagicNumber = MagicNumber,
            ByteOrderMarker = 0xFEFF,
            Version = 0x0100,
            ChunkCount = 1,
            HeaderLength = 0x10
        };

        bw.Pad(header.HeaderLength);

        Palettes.WriteTo(bw);

        if (PaletteCollectionMap != null)
        {
            header.ChunkCount = 2;
            PaletteCollectionMap.WriteTo(bw);
        }

        var endOffset = bw.BaseStream.Position;
        header.FileLength = (uint)(endOffset - initOffset);
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
    }


    public class PLTT
    {
        public struct Header
        {
            public const string MagicNumber = "TTLP";
            public uint TotalLength;
            public TexFormat Format;
            public ushort Unknown1;
            public uint Unknown2;
            public int DataLength;
            public const int DataStartOffset = 0x10;

            public Header(BinaryReader br)
            {
                var magicNumber = br.ReadMagicNumber();
                if (magicNumber != MagicNumber)
                {
                    throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                }

                TotalLength = br.ReadUInt32();
                Format = (TexFormat)br.ReadUInt16();
                Unknown1 = br.ReadUInt16();
                if (Unknown1 != 0)
                {
                    throw new InvalidDataException($"PLTT unknown1 is not zero, mayday! mayday!");
                }
                Unknown2 = br.ReadUInt32();
                if (Unknown2 != 0)
                {
                    throw new InvalidDataException($"PLTT unknown2 is not zero, mayday! mayday!");
                }
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
                bw.Write((ushort)Format);
                bw.Write(Unknown1);
                bw.Write(Unknown2);
                bw.Write(DataLength);
                bw.Write(DataStartOffset);
            }
        }

        public PLTT(BinaryReader br)
        {
            var header = new Header(br);
            Format = header.Format;
            Palette = PaletteUtil.Decompress(br.ReadBytes(header.DataLength));
        }

        public void WriteTo(BinaryWriter bw)
        {
            long initOffset = bw.BaseStream.Position;

            var header = new Header
            {
                Format = Format,
            };

            bw.Pad(Header.DataStartOffset + 8); // the data start offset is relative to the end of the generic chunk header which is magic num and total length i.e. 8

            var data = PaletteUtil.Compress(Palette);
            header.DataLength = data.Length;
            bw.Write(data);

            var endOffset = bw.BaseStream.Position;
            header.TotalLength = (uint)(endOffset - initOffset);
            bw.BaseStream.Position = initOffset;
            header.WriteTo(bw);

            bw.BaseStream.Position = endOffset;
        }

        public TexFormat Format { get; set; }
        public Rgb15[] Palette { get; set; }
    }

    public class PCMP
    {
        public struct Header
        {
            public const string MagicNumber = "PMCP";
            public int TotalLength;
            public ushort NumberOfPalettes;
            public const ushort Beef = 0xBEEF;
            public const int DataStartOffset = 0x8;

            public Header(BinaryReader br)
            {
                var magicNumber = br.ReadMagicNumber();
                if (magicNumber != MagicNumber)
                {
                    throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                }

                TotalLength = br.ReadInt32();
                NumberOfPalettes = br.ReadUInt16();
                var beef = br.ReadUInt16();
                // NB: BEEF often indicates dead memory
                if (beef != Beef)
                {
                    throw new InvalidDataException($"Unexpected PORK '0x{beef:X}' (expected 0xBEEF)");
                }

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
                bw.Write(NumberOfPalettes);
                bw.Write(Beef);
                bw.Write(DataStartOffset);
            }
        }

        public List<ushort> Palettes { get; set; }

        public PCMP(BinaryReader br)
        {
            var header = new Header(br);

            Palettes = new List<ushort>();
            for (int i = 0; i < header.NumberOfPalettes; i++)
            {
                Palettes.Add(br.ReadUInt16());
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            var header = new Header
            {
                NumberOfPalettes = (ushort)Palettes.Count,
                TotalLength = 8 + Header.DataStartOffset + (2 * Palettes.Count) 
            };

            header.WriteTo(bw);

            foreach (var i in Palettes)
            {
                bw.Write(i);
            }
        }
    }

}