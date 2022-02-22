using System.IO;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// Texture data. The one chunk of <see cref="BTX0"/>.
/// WARNING: Currently only supports writing to an existing file
/// </summary>
public class TEX0
{
    public struct Header
    {
        public const string MagicNumber = "TEX0";
        public uint TotalLength;

        public int TextureDataLength;
        public ushort TextureInfoOffset;
        public uint TextureDataOffset;

        public int TextureCompressedDataLength;
        public ushort TextureCompressedInfoOffset;
        public uint TextureCompressedDataOffset;
        public uint TextureCompressedInfoDataOffset;

        public int PaletteDataLength;
        public uint PaletteInfoOffset;
        public uint PaletteDataOffset;

        public Header(BinaryReader br)
        {
            var magicNumber = br.ReadMagicNumber();
            if (magicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
            }
            TotalLength = br.ReadUInt32();

            var padding = br.ReadUInt32();

            TextureDataLength = br.ReadUInt16() << 3;
            TextureInfoOffset = br.ReadUInt16();
            padding += br.ReadUInt32();
            TextureDataOffset = br.ReadUInt32();
            padding +=  br.ReadUInt32();

            TextureCompressedDataLength = br.ReadUInt16() << 3;
            TextureCompressedInfoOffset = br.ReadUInt16();
            padding += br.ReadUInt32();
            TextureCompressedDataOffset = br.ReadUInt32();
            TextureCompressedInfoDataOffset = br.ReadUInt32();

            padding += br.ReadUInt32();


            PaletteDataLength = br.ReadInt32() << 3;
            PaletteInfoOffset = br.ReadUInt32();
            PaletteDataOffset = br.ReadUInt32();


            if (padding != 0)
            {
                throw new InvalidDataException("In texture what was expected to be padding was not 0");
            }
        }
    }

    public TEX0(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        HeaderInstance = new Header(br);
        br.BaseStream.Position = initOffset + HeaderInstance.TextureDataOffset;
        PixelMap = RawChar.Decompress(br.ReadBytes(HeaderInstance.TextureDataLength));
        br.BaseStream.Position = initOffset + HeaderInstance.PaletteDataOffset;
        var palLen = HeaderInstance.PaletteDataLength / 2;
        Palette1 = RawPalette.Decompress(br.ReadBytes(palLen));
        Palette2 = RawPalette.Decompress(br.ReadBytes(palLen));
    }
    public Header HeaderInstance { get; set; }
    public byte[] PixelMap { get; set; }
    public Rgb15[] Palette1 { get; set; }
    public Rgb15[] Palette2 { get; set; }

   
    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        bw.BaseStream.Position = initOffset + HeaderInstance.TextureDataOffset;
        var compressedPixels = RawChar.Compress(PixelMap);
        bw.Write(compressedPixels);
        bw.BaseStream.Position = initOffset + HeaderInstance.PaletteDataOffset;
        bw.Write(RawPalette.Compress(Palette1));
        bw.Write(RawPalette.Compress(Palette2));
    }
}
