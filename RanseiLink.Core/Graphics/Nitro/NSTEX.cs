using RanseiLink.Core.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Graphics
{

    /// <summary>
    /// Texture data. The one chunk of <see cref="NSBTX"/>.
    /// WARNING: Currently only supports writing to an existing file
    /// </summary>
    public class NSTEX
    {
        public const string MagicNumber = "TEX0";

        private struct TexInfo
        {
            public const int Length = 16;

            public uint VramKey;
            public int DataSize;
            public ushort DictOffset;
            public ushort Flag;
            public ushort Nothing;
            public uint DataOffset;

            public TexInfo(BinaryReader br)
            {
                VramKey = br.ReadUInt32();
                DataSize = br.ReadUInt16() << 3;
                DictOffset = br.ReadUInt16();
                Flag = br.ReadUInt16();
                Nothing = br.ReadUInt16();
                DataOffset = br.ReadUInt32();
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(VramKey);
                bw.Write((ushort)(DataSize >> 3));
                bw.Write(DictOffset);
                bw.Write(Flag);
                bw.Write(Nothing);
                bw.Write(DataOffset);
            }
        }

        private struct Tex4x4Info
        {
            public const int Length = 20;

            public uint VramKey;
            public int DataSize;
            public ushort DictOffset;
            public ushort Flag;
            public ushort Nothing;
            public uint DataOffset;
            public uint OffsetTexPalIndex;

            public Tex4x4Info(BinaryReader br)
            {
                VramKey = br.ReadUInt32();
                DataSize = br.ReadUInt16() << 3;
                DictOffset = br.ReadUInt16();
                Flag = br.ReadUInt16();
                Nothing = br.ReadUInt16();
                DataOffset = br.ReadUInt32();
                OffsetTexPalIndex = br.ReadUInt32();
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(VramKey);
                bw.Write((ushort)(DataSize >> 3));
                bw.Write(DictOffset);
                bw.Write(Flag);
                bw.Write(Nothing);
                bw.Write(DataOffset);
                bw.Write(OffsetTexPalIndex);
            }
        }

        private struct PalInfo
        {
            public const int Length = 16;

            public uint VramKey;
            public int DataSize;
            public ushort Flag;
            public ushort DictOffset;
            public ushort Nothing;
            public uint DataOffset;

            public PalInfo(BinaryReader br)
            {
                VramKey = br.ReadUInt32();
                DataSize = br.ReadUInt16() << 3;
                Flag = br.ReadUInt16();
                DictOffset = br.ReadUInt16();
                Nothing = br.ReadUInt16();
                DataOffset = br.ReadUInt32();
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(VramKey);
                bw.Write((ushort)(DataSize >> 3));
                bw.Write(Flag);
                bw.Write(DictOffset);
                bw.Write(Nothing);
                bw.Write(DataOffset);
            }
        }

        private class TexRadixData : IRadixData
        {
            public int Offset;

            public bool RepeatX;
            public bool RepeatY;
            public bool FlipX;
            public bool FlipY;
            public int Width;
            public int Height;
            public TexFormat Format;
            public bool Color0Transparent;

            public ushort Length => 8;

            public void ReadFrom(BinaryReader br)
            {
                Offset = br.ReadUInt16() << 3;

                var bitField = br.ReadUInt16();
                RepeatX = (bitField >> 0 & 1) == 1;
                RepeatY = (bitField >> 1 & 1) == 1;
                FlipX = (bitField >> 2 & 1) == 1;
                FlipY = (bitField >> 3 & 1) == 1;
                Width = 8 << (bitField >> 4 & 0b111);
                Height = 8 << (bitField >> 7 & 0b111);
                Format = (TexFormat)(bitField >> 10 & 0b111);
                Color0Transparent = (bitField >> 13 & 1) == 1;
                var unknownBits = bitField >> 14 & 0b11;
                if (unknownBits != 0)
                {
                    throw new System.Exception("Unknown bits are not always zero!!!!!");
                }

                var bitField2 = br.ReadInt32();
                var width2 = bitField2 & 0x7FF;
                if (width2 != Width)
                {
                    throw new System.Exception("Width2 != width in texRadixData, please report the file this occurred with");
                }
                var height2 = bitField2 >> 11 & 0x7FF;
                if (height2 != Height)
                {
                    throw new System.Exception("height2 != Height in texRadixData, please report the file this occurred with");
                }
                var always1 = bitField2 & int.MinValue;
                if (always1 != int.MinValue)
                {
                    throw new System.Exception("it isn't always 1 (0_0)");
                }
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write((ushort)(Offset >> 3));

                if (Width > 1024 || !BitUtil.TryReverseLogicalShiftLeft(8, Width, out int storedWidth))
                {
                    throw new System.Exception($"Invalid texture width '{Width}'. Valid values 8, 16, 32, 64, 128, 256, 512, 1024");
                }
                if (Height > 1024 || !BitUtil.TryReverseLogicalShiftLeft(8, Height, out int storedHeight))
                {
                    throw new System.Exception($"Invalid texture height '{Height}'. Valid values 8, 16, 32, 64, 128, 256, 512, 1024");
                }

                var bitField =
                      (RepeatX ? 1 : 0)
                    | (RepeatY ? 1 : 0) << 1
                    | (FlipX ? 1 : 0) << 2
                    | (FlipY ? 1 : 0) << 3
                    | (storedWidth & 0b111) << 4
                    | (storedHeight & 0b111) << 7
                    | ((int)Format & 0b111) << 10
                    | (Color0Transparent ? 1 : 0) << 13;

                bw.Write((ushort)bitField);

                int bitField2 = Width & 0x7FF | (Height & 0x7FF) << 11 | int.MinValue;
                bw.Write(bitField2);
            }
        }

        private class PalRadixData : IRadixData
        {
            public int Offset;

            public ushort Length => 4;

            public void ReadFrom(BinaryReader br)
            {
                Offset = br.ReadUInt16() << 3;
                var unknown = br.ReadUInt16();
                if (unknown != 0)
                {
                    throw new System.Exception("PaletteRadixDict unknown != 0");
                }
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write((ushort)(Offset >> 3));
                bw.Write((ushort)0);
            }
        }
        
        public NSTEX()
        {

        }

        private const int _bitsPerByte = 8;

        public void WriteTo(BinaryWriter bw)
        {
            var initOffset = bw.BaseStream.Position;

            // skip header and radix dicts for now
            var texDictOffset = NitroChunkHeader.Length + TexInfo.Length + Tex4x4Info.Length + PalInfo.Length;
            var palDictOffset = texDictOffset + RadixDict<TexRadixData>.CalculateLength(Textures.Count);
            bw.Pad(palDictOffset + RadixDict<PalRadixData>.CalculateLength(Palettes.Count));

            // create header, infos, and dicts
            var header = new NitroChunkHeader { MagicNumber = MagicNumber };
            var texInfo = new TexInfo();
            var tex4x4Info = new Tex4x4Info();
            var palInfo = new PalInfo();
            var texRadixDict = new RadixDict<TexRadixData>();
            var palRadixDict = new RadixDict<PalRadixData>();
            texInfo.DictOffset = tex4x4Info.DictOffset = (ushort)texDictOffset;
            palInfo.DictOffset = (ushort)palDictOffset;

            // write textures
            var texStartOffset = bw.BaseStream.Position;
            texInfo.DataOffset = (uint)(texStartOffset - initOffset);

            // write 4x4 textures to memory stream to begin with because they are written in the file after the other textures
            BinaryWriter buffer4x4 = null;
            if (Textures.Any(x => x.Format == TexFormat.Comp4x4))
            {
                buffer4x4 = new BinaryWriter(new MemoryStream());
            }
            foreach (var t in Textures)
            {
                texRadixDict.Names.Add(t.Name);
                var texdata = new TexRadixData
                {
                    RepeatX = t.RepeatX,
                    RepeatY = t.RepeatY,
                    FlipX = t.FlipX,
                    FlipY = t.FlipY,
                    Width = t.Width,
                    Height = t.Height,
                    Format = t.Format,
                    Color0Transparent = t.Color0Transparent
                };

                var pixels = t.TextureData;
                if (t.Format == TexFormat.Pltt16)
                {
                    pixels = RawChar.Compress(pixels);
                }

                if (t.Format == TexFormat.Comp4x4)
                {
                    texdata.Offset = (int)buffer4x4.BaseStream.Position;
                }
                else
                {
                    texdata.Offset = (int)(bw.BaseStream.Position - texStartOffset);
                    bw.Write(pixels);
                }
                texRadixDict.Data.Add(texdata);
            }
            texInfo.DataSize = (int)(bw.BaseStream.Position - texStartOffset);

            // write textures 4x4
            var tex4x4StartOffset = bw.BaseStream.Position;
            tex4x4Info.DataOffset = (uint)(tex4x4StartOffset - initOffset);
            tex4x4Info.OffsetTexPalIndex = tex4x4Info.DataOffset; // what is this value?
            if (buffer4x4 != null)
            {
                buffer4x4.BaseStream.Position = 0;
                buffer4x4.BaseStream.CopyTo(bw.BaseStream);
                buffer4x4.Dispose();
            }
            tex4x4Info.DataSize = (int)(bw.BaseStream.Position - tex4x4StartOffset);

            // write palettes
            var palStartOffset = bw.BaseStream.Position;
            palInfo.DataOffset = (uint)(palStartOffset - initOffset);
            foreach (var p in Palettes)
            {
                palRadixDict.Names.Add(p.Name);
                palRadixDict.Data.Add(new PalRadixData
                {
                    Offset = (int)(bw.BaseStream.Position - palStartOffset)
                });
                bw.Write(RawPalette.Compress(p.PaletteData));
            }
            palInfo.DataSize = (int)(bw.BaseStream.Position - palStartOffset);

            // write headers, info, and radix dicts
            var endOffset = bw.BaseStream.Position;
            header.ChunkLength = (uint)(endOffset - initOffset);

            bw.BaseStream.Position = initOffset;

            header.WriteTo(bw);
            texInfo.WriteTo(bw);
            tex4x4Info.WriteTo(bw);
            palInfo.WriteTo(bw);
            texRadixDict.WriteTo(bw);
            palRadixDict.WriteTo(bw);

            bw.BaseStream.Position = endOffset;
        }

        public NSTEX(BinaryReader br)
        {
            var initOffset = br.BaseStream.Position;
            var header = new NitroChunkHeader(br);
            if (header.MagicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' (expected: {MagicNumber})");
            }
            var texInfo = new TexInfo(br);
            var tex4x4Info = new Tex4x4Info(br);
            var palInfo = new PalInfo(br);
            var texRadixDict = new RadixDict<TexRadixData>(br);
            var palRadixDict = new RadixDict<PalRadixData>(br);

            for (int i = 0; i < texRadixDict.Names.Count; i++)
            {
                var data = texRadixDict.Data[i];
                var name = texRadixDict.Names[i];
                byte[] pixels;

                br.BaseStream.Position = data.Format == TexFormat.Comp4x4
                    ? initOffset + tex4x4Info.DataOffset + data.Offset
                    : initOffset + texInfo.DataOffset + data.Offset;

                pixels = br.ReadBytes(data.Width * data.Height * data.Format.BitsPerPixel() / _bitsPerByte);

                if (data.Format == TexFormat.Pltt16)
                {
                    pixels = RawChar.Decompress(pixels);
                }

                Textures.Add(new Texture
                {
                    Name = name,
                    TextureData = pixels,

                    RepeatX = data.RepeatX,
                    RepeatY = data.RepeatY,
                    FlipX = data.FlipX,
                    FlipY = data.FlipY,
                    Width = data.Width,
                    Height = data.Height,
                    Format = data.Format,
                    Color0Transparent = data.Color0Transparent,
                });
            }

            for (int i = 0; i < palRadixDict.Names.Count; i++)
            {
                var data = palRadixDict.Data[i];
                var name = palRadixDict.Names[i];

                var offsetStart = initOffset + palInfo.DataOffset + data.Offset;
                long offsetEnd;
                if (i == (palRadixDict.Names.Count - 1))
                {
                    offsetEnd = initOffset + header.ChunkLength;
                }
                else
                {
                    offsetEnd = initOffset + palInfo.DataOffset + palRadixDict.Data[i + 1].Offset;
                }
                br.BaseStream.Position = offsetStart;
                var palette = RawPalette.Decompress(br.ReadBytes((int)(offsetEnd - offsetStart)));
                Palettes.Add(new Palette
                {
                    Name = name,
                    PaletteData = palette,
                });
            }
        }

        public class Texture
        {
            public string Name { get; set; }

            public byte[] TextureData { get; set; }

            public bool RepeatX { get; set; }
            public bool RepeatY { get; set; }
            public bool FlipX { get; set; }
            public bool FlipY { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public TexFormat Format { get; set; }
            public bool Color0Transparent { get; set; }
        }

        public class Palette
        {
            public string Name { get; set; }

            public Rgb15[] PaletteData { get; set; }

        }

        public List<Texture> Textures { get; set; } = new List<Texture>();
        public List<Palette> Palettes { get; set; } = new List<Palette>();
    }
}