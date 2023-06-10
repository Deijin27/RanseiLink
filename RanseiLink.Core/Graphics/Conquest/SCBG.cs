using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Graphics
{
    public class SCBGCollection
    {
        public List<SCBG?> Items { get; set; }

        public SCBGCollection(List<SCBG?> items)
        {
            Items = items;
        }

        public static SCBGCollection Load(string scbgDataFile, string scbgInfoFile)
        {
            var scbgs = new List<SCBG?>();
            using (var dataBr = new BinaryReader(File.OpenRead(scbgDataFile)))
            {
                using (var infoBr = new BinaryReader(File.OpenRead(scbgInfoFile)))
                {
                    int numItems = infoBr.ReadInt32();
                    int _ = infoBr.ReadInt32(); // length of the largest file
                    for (int i = 0; i < numItems; i++)
                    {
                        var offset = infoBr.ReadUInt32();
                        var len = infoBr.ReadInt32();
                        if (len == 0)
                        {
                            scbgs.Add(null);
                        }
                        else
                        {
                            dataBr.BaseStream.Position = offset;
                            scbgs.Add(SCBG.Load(dataBr));
                        }
                    }

                    return new SCBGCollection(scbgs);
                }
            }
        }

        public void Save(string scbgDataFile, string scbgInfoFile)
        {
            if (Items.Count == 0)
            {
                throw new Exception("Cannot save an empty collection");
            }
            using (var dataBw = new BinaryWriter(File.Create(scbgDataFile)))
            {
                using (var infoBw = new BinaryWriter(File.Create(scbgInfoFile)))
                {
                    // header
                    infoBw.Write(Items.Count);
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
                        dataBw.Pad(0x10 - (len % 0x10));
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

        public static SCBGCollection LoadPngs(string inputFolder, bool tiled)
        {
            return LoadPngs(Directory.GetFiles(inputFolder, "*.png"), tiled);
        }

        public static SCBGCollection LoadPngs(string[] files, bool tiled)
        {
            var scbgFiles = (
                from filePath in files
                let fileName = Path.GetFileNameWithoutExtension(filePath)
                where int.TryParse(fileName, out var _)
                let num = int.Parse(fileName)
                select (filePath, num))
                .ToArray();

            int maxNum = scbgFiles.Max(i => i.num);
            var scbg = new SCBG?[maxNum + 1];
            Parallel.For(0, scbgFiles.Length, i =>
            {
                var (file, num) = scbgFiles[i];
                if (new FileInfo(file).Length == 0)
                {
                    Console.WriteLine("File of len 0 hit");
                    scbg[num] = null;
                }
                else
                {
                    scbg[num] = SCBG.LoadPng(file, tiled);
                }
            });
            return new SCBGCollection(scbg.ToList());
        }

        public void SaveAsPngs(string outputFolder, bool tiled)
        {
            Directory.CreateDirectory(outputFolder);
            Parallel.For(0, Items.Count, i =>
            {
                var scbg = Items[i];
                string saveFile = Path.Combine(outputFolder, $"{i.ToString().PadLeft(4, '0')}.png");
                if (scbg == null)
                {
                    File.Create(saveFile).Dispose(); // create an empty file
            }
                else
                {
                    scbg.SaveAsPng(saveFile, tiled: tiled);
                }
            });
        }
    }


    /// <summary>
    /// screen background
    /// </summary>
    public class SCBG
    {
        public const string MagicNumber = "GBCS";
        public const int PaletteDataLength = 0x200;
        public const int HeaderLength = 8;
        public const int PaddingLength = 8;

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public Rgb15[] Palette { get; set; }
        public byte[] Pixels { get; set; }

        private SCBG(ushort width, ushort height, Rgb15[] palette, byte[] pixels)
        {
            Width = width;
            Height = height;
            Palette = palette;
            Pixels = pixels;
        }

        public static SCBG Load(BinaryReader br)
        {
            var magicNumber = br.ReadMagicNumber();
            if (magicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
            }

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            var palette = RawPalette.Decompress(br.ReadBytes(PaletteDataLength));
            var pixels = br.ReadBytes(width * height);
            return new SCBG(width, height, palette, pixels);
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.WriteMagicNumber(MagicNumber);
            bw.Write(Width);
            bw.Write(Height);
            var posBeforePal = bw.BaseStream.Position;
            bw.Write(RawPalette.Compress(Palette));
            var posAfterPal = bw.BaseStream.Position;
            int palLen = (int)(posAfterPal - posBeforePal);
            if (palLen < PaletteDataLength)
            {
                bw.Pad(PaletteDataLength - palLen);
            }
            bw.Write(Pixels);
        }

        public void SaveAsPng(string saveFile, bool tiled)
        {
            ImageUtil.SpriteToPng(
                file: saveFile,
                new SpriteImageInfo(Pixels, RawPalette.To32bitColors(Palette), Width, Height),
                tiled: tiled,
                format: TexFormat.Pltt256
                );
        }

        public static SCBG LoadPng(string file, bool tiled)
        {
            var imageInfo = ImageUtil.SpriteFromPng(file, tiled, TexFormat.Pltt256, color0ToTransparent: true);

            return new SCBG
            (
                width: (ushort)imageInfo.Width,
                height: (ushort)imageInfo.Height,
                palette: RawPalette.From32bitColors(imageInfo.Palette),
                pixels: imageInfo.Pixels
            );
        }

    }
}