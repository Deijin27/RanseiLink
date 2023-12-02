using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Graphics
{
    public class STLCollection
    {
        public List<STL?> Items { get; set; }

        public STLCollection(List<STL?> items)
        {
            Items = items;
        }

        public static STLCollection Load(string stlDataFile, string stlInfoFile)
        {
            var stls = new List<STL?>();
            using (var dataBr = new BinaryReader(File.OpenRead(stlDataFile)))
            {
                using (var infoBr = new BinaryReader(File.OpenRead(stlInfoFile)))
                {
                    int numItems = infoBr.ReadInt32();
                    int _ = infoBr.ReadInt32(); // max file length
                    for (int i = 0; i < numItems; i++)
                    {
                        var offset = infoBr.ReadUInt32();
                        var len = infoBr.ReadInt32();
                        if (len == 0)
                        {
                            stls.Add(null);
                        }
                        else
                        {
                            dataBr.BaseStream.Position = offset;
                            stls.Add(STL.Load(dataBr));
                        }
                    }

                    return new STLCollection(stls);
                }
            }
        }

        public void Save(string stlDataFile, string stlInfoFile)
        {
            if (Items.Count == 0)
            {
                throw new Exception("Cannot save an empty collection");
            }
            using (var dataBw = new BinaryWriter(File.Create(stlDataFile)))
            {
                using (var infoBw = new BinaryWriter(File.Create(stlInfoFile)))
                {
                    // header
                    infoBw.Write(Items.Count);
                    // size of an item. idk why this is here because the items also list individual sizes
                    long maxLenPos = infoBw.BaseStream.Position;
                    infoBw.Pad(4);

                    int maxLen = 0;

                    foreach (var item in Items)
                    {
                        int initOffset = (int)dataBw.BaseStream.Position;
                        infoBw.Write(initOffset);
                        if (item != null)
                        {
                            item.WriteTo(dataBw);
                        }
                        int len = (int)dataBw.BaseStream.Position - initOffset;
                        infoBw.Write(len);
                        if (len > maxLen)
                        {
                            maxLen = len;
                        }
                    }

                    infoBw.BaseStream.Position = maxLenPos;
                    infoBw.Write(maxLen);
                }
            }
        }

        public static STLCollection LoadPngs(string inputFolder, NCER ncer, bool tiled)
        {
            return LoadPngs(Directory.GetFiles(inputFolder, "*.png"), ncer, tiled);
        }

        public static STLCollection LoadPngs(string[] files, NCER ncer, bool tiled)
        {
            var stlFiles = (
                from filePath in files
                let fileName = Path.GetFileNameWithoutExtension(filePath)
                where int.TryParse(fileName, out var _)
                let num = int.Parse(fileName)
                select (filePath, num))
                .ToArray();

            int maxNum = stlFiles.Max(i => i.num);
            var stls = new STL?[maxNum + 1];
            Parallel.For(0, stlFiles.Length, i =>
            {
                var (file, num) = stlFiles[i];
                if (new FileInfo(file).Length == 0)
                {
                    stls[num] = null;
                }
                else
                {
                    stls[num] = STL.LoadPng(ncer, file, tiled);
                }
            });
            return new STLCollection(stls.ToList());
        }

        public void SaveAsPngs(string outputFolder, NCER ncer, bool tiled)
        {
            Directory.CreateDirectory(outputFolder);
            Parallel.For(0, Items.Count, i =>
            {
                var stl = Items[i];
                string saveFile = Path.Combine(outputFolder, $"{i.ToString().PadLeft(4, '0')}.png");
                if (stl == null)
                {
                    File.Create(saveFile).Dispose(); // create an empty file
                                                     // the empty file creation is just a precaution in case the number of files matters in some cases.
                                                     // need a way to know if there is empty files at the end of the list of numbers.
            }
                else
                {
                    stl.SaveAsPng(ncer, saveFile, tiled: tiled);
                }
            });
        }
    }

    /// <summary>
    /// Still images of pokemon or warriors
    /// </summary>
    public class STL
    {
        public struct Header
        {
            public const int Length = 0x20;

            public int TotalLength;
            public uint Unknown1;
            public int Width;
            public int Height;
            public uint Unknown2;

            public Header(BinaryReader br)
            {
                TotalLength = br.ReadInt32();
                Unknown1 = br.ReadUInt32();
                if (Unknown1 != 0x_77_77_77_77)
                {
                    throw new InvalidDataException($"Unexpected value 0x{Unknown1:X} of STL Header {nameof(Unknown1)} (expected 0x{0x_77_77_77_77:X})");
                }
                Width = br.ReadInt32();
                Height = br.ReadInt32();
                Unknown2 = br.ReadUInt32();
                if (Unknown2 != 1)
                {
                    throw new InvalidDataException($"Unexpected value 0x{Unknown2:X} of STL Header {nameof(Unknown2)} (expected 0x{1})");
                }
                br.Skip(12);
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(TotalLength);
                bw.Write(Unknown1);
                bw.Write(Width);
                bw.Write(Height);
                bw.Write(Unknown2);
                bw.Pad(12);
            }
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public Rgb15[] Palette { get; set; }
        public byte[] Pixels { get; set; }

        public const int PaletteDataLength = 0x200;
        public const int PaddingLength = 0x60;

        private STL(int width, int height, Rgb15[] palette, byte[] pixels)
        {
            Width = width;
            Height = height;
            Palette = palette;
            Pixels = pixels;
        }

        public static STL Load(BinaryReader br)
        {
            var header = new Header(br);
            var palette = PaletteUtil.Decompress(br.ReadBytes(PaletteDataLength));
            var pixels = br.ReadBytes(header.Width * header.Height);
            br.Skip(PaddingLength);
            return new STL
            (
                width: header.Width,
                height: header.Height,
                palette: palette,
                pixels: pixels
            );
        }

        public void WriteTo(BinaryWriter bw)
        {
            var header = new Header
            {
                TotalLength = Header.Length + PaletteDataLength + (Width * Height) + PaddingLength,
                Unknown1 = 0x_77_77_77_77,
                Width = Width,
                Height = Height,
                Unknown2 = 1
            };

            header.WriteTo(bw);
            var posBeforePal = bw.BaseStream.Position;
            bw.Write(PaletteUtil.Compress(Palette));
            var posAfterPal = bw.BaseStream.Position;
            int palLen = (int)(posAfterPal - posBeforePal);
            if (palLen < PaletteDataLength)
            {
                bw.Pad(PaletteDataLength - palLen);
            }
            bw.Write(Pixels);
            bw.Pad(PaddingLength);
        }

        public void SaveAsPng(NCER ncer, string saveFile, bool tiled, bool debug = false)
        {
            const TexFormat format = TexFormat.Pltt256;
            CellImageUtil.SingleBankToPng(
                file: saveFile,
                bank: ncer.CellBanks.Banks[0],
                blockSize: ncer.CellBanks.BlockSize,
                new MultiPaletteImageInfo(Pixels, new PaletteCollection(Palette, format, true), Width, Height,
                    IsTiled: tiled,
                    Format: format),
                debug: debug
                );
        }

        public static STL LoadPng(NCER ncer, string pngFile, bool tiled)
        {
            var imageInfo = CellImageUtil.SingleBankFromPng(
                file: pngFile,
                bank: ncer.CellBanks.Banks[0],
                blockSize: ncer.CellBanks.BlockSize,
                tiled: tiled,
                format: TexFormat.Pltt256
                );

            if (imageInfo.Palette.Count != 1)
            {
                throw new Exception("Multi-palette STL not supported");
            }

            return new STL
            (
                width: imageInfo.Width,
                height: imageInfo.Height,
                pixels: imageInfo.Pixels,
                palette: PaletteUtil.From32bitColors(imageInfo.Palette[0])
            );
        }
    }
}