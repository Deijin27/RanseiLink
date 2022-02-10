using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Graphics;

public class SCBGCollection
{
    public List<SCBG> Items { get; set; }

    public static SCBGCollection Load(string scbgDataFile, string scbgInfoFile)
    {
        var scbgs = new List<SCBG>();
        using var dataBr = new BinaryReader(File.OpenRead(scbgDataFile));
        using var infoBr = new BinaryReader(File.OpenRead(scbgInfoFile));

        int numItems = infoBr.ReadInt32();
        int unused = infoBr.ReadInt32(); // length of one of the files, but then the individual files have lengths so it's not necessary
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

        return new SCBGCollection { Items = scbgs };
    }

    public void Save(string scbgDataFile, string scbgInfoFile)
    {
        if (Items.Count == 0)
        {
            throw new Exception("Cannot save an empty collection");
        }
        using var dataBw = new BinaryWriter(File.Create(scbgDataFile));
        using var infoBw = new BinaryWriter(File.Create(scbgInfoFile));

        // header
        infoBw.Write(Items.Count);
        // this is very weird
        infoBw.Write(SCBG.HeaderLength + SCBG.PaletteDataLength + (Items[0].Width * Items[0].Height) + Items[0].Height * (Items[0].Width - 0x10));

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
        }
    }

    public static SCBGCollection LoadPngs(string inputFolder, bool tiled = false)
    {
        var scbgFiles = (
            from filePath in Directory.GetFiles(inputFolder, "*.png")
            let fileName = Path.GetFileNameWithoutExtension(filePath)
            where int.TryParse(fileName, out var _)
            let num = int.Parse(fileName)
            select (filePath, num))
            .ToArray();

        int maxNum = scbgFiles.Max(i => i.num);
        var scbg = new SCBG[maxNum + 1];
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
        return new SCBGCollection { Items = scbg.ToList() };
    }

    public void SaveAsPngs(string outputFolder, bool tiled = false)
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

    private SCBG()
    {

    }

    public static SCBG Load(BinaryReader br)
    {
        var scbg = new SCBG();

        var magicNumber = br.ReadMagicNumber();
        if (magicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number '{magicNumber}'. (expected: {MagicNumber})");
        }
        scbg.Width = br.ReadUInt16();
        scbg.Height = br.ReadUInt16();

        scbg.Palette = RawPalette.Decompress(br.ReadBytes(PaletteDataLength));
        scbg.Pixels = br.ReadBytes(scbg.Width * scbg.Height);

        return scbg;
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

    public void SaveAsPng(string saveFile, bool tiled = false)
    {
        ImageUtil.SaveAsPng(
            file: saveFile,
            new ImageInfo(Pixels, RawPalette.To32bitColors(Palette), Width, Height),
            tiled: tiled
            );
    }

    public static SCBG LoadPng(string file, bool tiled = false)
    {
        var imageInfo = ImageUtil.LoadPng(file, tiled);

        return new SCBG
        {
            Width = (ushort)imageInfo.Width,
            Height = (ushort)imageInfo.Height,
            Palette = RawPalette.From32bitColors(imageInfo.Palette),
            Pixels = imageInfo.Pixels,
        };
    }

}
