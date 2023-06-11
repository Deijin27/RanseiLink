using FluentResults;
using RanseiLink.Core.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RanseiLink.Core.RomFs;

public class BannerInfo
{
    public string JapaneseTitle { get; set; } = string.Empty;
    public string EnglishTitle { get; set; } = string.Empty;
    public string FrenchTitle { get; set; } = string.Empty;
    public string GermanTitle { get; set; } = string.Empty;
    public string ItalianTitle { get; set; } = string.Empty;
    public string SpanishTitle { get; set; } = string.Empty;
}

public class Banner : BannerInfo
{
    private Rgb15[] _imagePalette;

    public ushort Version { get; set; }
    public ushort CRC { get; private set; }
    public bool CRCCorrect { get; set; }
    public byte[] Reserved { get; set; }

    public byte[] ImagePixels { get; set; }
    public Rgb15[] ImagePalette
    {
        get => _imagePalette;
        set
        {
            if (value.Length != 16)
            {
                Rgb15[] palette = new Rgb15[16];
                value.CopyTo(palette, 0);
                _imagePalette = palette;
            }
            else
            {
                _imagePalette = value;
            }
        }
    }

    public Banner(BinaryReader br)
    {
        Version = br.ReadUInt16();
        CRC = br.ReadUInt16();
        Reserved = br.ReadBytes(0x1C);

        var crcDataStart = br.BaseStream.Position;
        var crc16 = new Checksum.CRC16(Checksum.CRC16.IBMReversedPolynomial);
        ushort crc = crc16.Calculate(br.ReadBytes(0x820));
        CRCCorrect = CRC == crc;
        br.BaseStream.Position = crcDataStart;
        ImagePixels = RawChar.Decompress(br.ReadBytes(0x200));
        _imagePalette = RawPalette.Decompress(br.ReadBytes(0x20));

        JapaneseTitle = ReadTitle(br);
        EnglishTitle = ReadTitle(br);
        FrenchTitle = ReadTitle(br);
        GermanTitle = ReadTitle(br);
        ItalianTitle = ReadTitle(br);
        SpanishTitle = ReadTitle(br);
    }

    public void WriteTo(BinaryWriter bw, BinaryReader br)
    {
        bw.Write(Version);
        var crcOffset = bw.BaseStream.Position;
        bw.Pad(2);
        bw.Write(Reserved);
        var crcDataStart = bw.BaseStream.Position;
        bw.Write(RawChar.Compress(ImagePixels));
        bw.Write(RawPalette.Compress(ImagePalette));

        WriteTitle(bw, JapaneseTitle);
        WriteTitle(bw, EnglishTitle);
        WriteTitle(bw, FrenchTitle);
        WriteTitle(bw, GermanTitle);
        WriteTitle(bw, ItalianTitle);
        WriteTitle(bw, SpanishTitle);

        var end = bw.BaseStream.Position;

        bw.BaseStream.Position = crcDataStart;
        var crc16 = new Checksum.CRC16(Checksum.CRC16.IBMReversedPolynomial);
        ushort crc = crc16.Calculate(br.ReadBytes(0x820));
        bw.BaseStream.Position = crcOffset;
        bw.Write(crc);

        bw.BaseStream.Position = end;
    }

    private static string ReadTitle(BinaryReader br)
    {
        return Encoding.Unicode.GetString(br.ReadBytes(0x100))
            .Replace("\x0", "");
    }

    private static void WriteTitle(BinaryWriter bw, string title)
    {
        byte[] data = Encoding.Unicode.GetBytes(title);
        if (data.Length > 0x100)
        {
            data = data.Take(0x100).ToArray();
        }
        byte[] finalData = new byte[0x100];
        data.CopyTo(finalData, 0);
        bw.Write(finalData);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{nameof(Version)}: {Version}");
        sb.AppendLine($"{nameof(CRC)}: {CRC}");
        sb.AppendLine($"{nameof(CRCCorrect)}: {CRCCorrect}");

        sb.AppendLine($"{nameof(JapaneseTitle)}: {JapaneseTitle}");
        sb.AppendLine($"{nameof(EnglishTitle)}: {EnglishTitle}");
        sb.AppendLine($"{nameof(FrenchTitle)}: {FrenchTitle}");
        sb.AppendLine($"{nameof(GermanTitle)}: {GermanTitle}");
        sb.AppendLine($"{nameof(ItalianTitle)}: {ItalianTitle}");
        sb.AppendLine($"{nameof(SpanishTitle)}: {SpanishTitle}");


        return sb.ToString();
    }
}

public static class BannerExtensions
{
    public static void SaveInfoToXml(this BannerInfo banner, string file)
    {
        var doc = new XDocument(
        new XElement("BannerInfo",
            new XElement(nameof(banner.JapaneseTitle), banner.JapaneseTitle),
            new XElement(nameof(banner.EnglishTitle), banner.EnglishTitle),
            new XElement(nameof(banner.FrenchTitle), banner.FrenchTitle),
            new XElement(nameof(banner.GermanTitle), banner.GermanTitle),
            new XElement(nameof(banner.ItalianTitle), banner.ItalianTitle),
            new XElement(nameof(banner.SpanishTitle), banner.SpanishTitle)
        ));

        doc.Save(file);
    }

    public static Result TryLoadInfoFromXml(this BannerInfo banner, string file)
    {
        try
        {
            var doc = XDocument.Load(file);
            var root = doc.Element("BannerInfo");
            if (root == null)
            {
                return Result.Fail("Banner missing root element 'BannerInfo'");
            }
            var japaneseElement = root.Element(nameof(banner.JapaneseTitle));
            if (japaneseElement != null)
            {
                banner.JapaneseTitle = japaneseElement.Value;
            }
            var englishElement = root.Element(nameof(banner.EnglishTitle));
            if (englishElement != null)
            {
                banner.EnglishTitle = englishElement.Value;
            }
            var frenchElement = root.Element(nameof(banner.FrenchTitle));
            if (frenchElement != null)
            {
                banner.FrenchTitle = frenchElement.Value;
            }
            var germanElement = root.Element(nameof(banner.GermanTitle));
            if (germanElement != null)
            {
                banner.GermanTitle = germanElement.Value;
            }
            var italianElement = root.Element(nameof(banner.ItalianTitle));
            if (italianElement != null)
            {
                banner.ItalianTitle = italianElement.Value;
            }
            var spanishElement = root.Element(nameof(banner.SpanishTitle));
            if (spanishElement != null)
            {
                banner.SpanishTitle = spanishElement.Value;
            }
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.ToString());
        }
        
    }

    public static void SaveImageToPng(this Banner banner, string file)
    {
        var imageInfo = new SpriteImageInfo(
            Pixels: banner.ImagePixels,
            Palette: RawPalette.To32bitColors(banner.ImagePalette),
            Width: 32,
            Height: 32
            );

        ImageUtil.SpriteToPng(file, imageInfo, tiled: true, format: TexFormat.Pltt16);
    }

    public static Result TryLoadImageFromPng(this Banner banner, string file)
    {
        var imageInfo = ImageUtil.SpriteFromPng(file, tiled: true, format: TexFormat.Pltt16, color0ToTransparent: true);
        if (imageInfo.Width != 32 || imageInfo.Height != 32)
        {
            return Result.Fail("Invalid image dimensions. Should be 32x32 pixels");
        }
        if (imageInfo.Palette.Length > 16)
        {
            return Result.Fail("Invalid image palette. Should have at most 16 colors. You could use 'simplify palette' command");
        }

        banner.ImagePalette = RawPalette.From32bitColors(imageInfo.Palette);
        banner.ImagePixels = imageInfo.Pixels;

        return Result.Ok();
    }

    /// <summary>
    /// Set all titles to the same value
    /// </summary>
    public static void SetAllTitles(this BannerInfo banner, string value)
    {
        banner.JapaneseTitle = value;
        banner.EnglishTitle = value;
        banner.FrenchTitle = value;
        banner.GermanTitle = value;
        banner.ItalianTitle = value;
        banner.SpanishTitle = value;
    }
}
