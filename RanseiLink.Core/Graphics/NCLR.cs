using System.IO;

namespace RanseiLink.Core.Graphics
{
    public class NCLR
    {
        public const string MagicNumber = "RLCN";
        public static readonly string[] FileExtensions = new[] { ".nclr", ".rlcn" };

        public static NCLR Load(string file)
        {
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                return new NCLR(br);
            } 
        }

        public NCLR(BinaryReader br)
        {
            long initOffset = br.BaseStream.Position;

            // first a typical file header
            var header = new GenericFileHeader(br);

            if (header.MagicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
            }

            // read 
            Palettes = new PLTT(br);

            br.BaseStream.Position = initOffset + header.FileLength;
        }

        public PLTT Palettes { get; set; }

        public class PLTT
        {
            public struct Header
            {
                public const string MagicNumber = "TTLP";
                public uint TotalLength;
                public BitsPerPixel BitsPerPixel;
                public ushort Unknown1;
                public uint Unknown2;
                public int DataLength;
                public uint DataOffset;

                public Header(BinaryReader br)
                {
                    var magicNumber = br.ReadMagicNumber();
                    if (magicNumber != MagicNumber)
                    {
                        throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
                    }

                    TotalLength = br.ReadUInt32();
                    BitsPerPixel = (BitsPerPixel)br.ReadUInt16();
                    Unknown1 = br.ReadUInt16();
                    Unknown2 = br.ReadUInt32();
                    DataLength = br.ReadInt32();
                    DataOffset = br.ReadUInt32();
                }
            }

            public PLTT(BinaryReader br)
            {
                var initOffset = br.BaseStream.Position;
                var header = new Header(br);
                br.BaseStream.Position = initOffset + 8 + header.DataOffset;
                Palette = RawPalette.Decompress(br.ReadBytes(header.DataLength));
            }

            public Rgb15[] Palette { get; set; }
        }

    }
}