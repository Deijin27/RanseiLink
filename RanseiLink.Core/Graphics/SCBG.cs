using System.Collections.Generic;
using System.IO;
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

    public void SaveAsPng(string saveFile, bool tiled = false)
    {
        ImageUtil.SaveAsPng(
            file: saveFile,
            pixelArray: Pixels,
            palette: RawPalette.To32bitColors(Palette),
            width: Width,
            tiled: tiled
            );
    }

}
