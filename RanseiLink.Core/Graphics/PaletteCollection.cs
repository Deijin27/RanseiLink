using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Graphics;

public class Palette : List<Rgba32>
{
    public Palette(int capacity, bool color0Transparent) : base(capacity)
    {
        if (color0Transparent)
        {
            Add(Color.Transparent);
        }
    }

    public Palette(TexFormat format, bool color0Transparent) : this(format.PaletteSize(), color0Transparent)
    {

    }

    public Palette(Rgb15[] colors, bool color0Transparent) : this(colors.Length, color0Transparent)
    {
        int initColId = 0;
        if (color0Transparent)
        {
            initColId++;
        }
        for (int colId = initColId; colId < colors.Length; colId++)
        {
            Add(PaletteUtil.To32BitColor(colors[colId]));
        }
    }
}

public class PaletteCollection : List<Palette>
{
    public PaletteCollection(int numPalettes, TexFormat format, bool color0Transparent)
    {
        for (int i = 0; i < numPalettes; i++)
        {
            Add(new(format.PaletteSize(), color0Transparent));
        }
    }

    public PaletteCollection(IReadOnlyList<CellBank> banks, TexFormat format, bool color0Transparent)
        : this(banks.SelectMany(x => x).Max(x => x.IndexPalette) + 1, format, color0Transparent)
    {

    }

    public PaletteCollection(CellBank bank, TexFormat format, bool color0Transparent)
        : this(bank.Max(x => x.IndexPalette) + 1, format, color0Transparent)
    {

    }

    public void SetColor0(Color color)
    {
        foreach (var palette in this)
        {
            palette[0] = color;
        }
    }

    public PaletteCollection(Rgb15[] palette, TexFormat format, bool color0Transparent)
    {
        var paletteSize = format.PaletteSize();
        if (paletteSize == 0)
        {
            throw new System.Exception($"Handling of palette format {format} not implemented");
        }
        if (palette.Length % paletteSize != 0)
        {
            throw new System.Exception($"Palette colors ({palette.Length}) are not divisible by expected palette length {paletteSize}");
        }
        var paletteCount = palette.Length / format.PaletteSize();
        var cumulativeOffset = 0;
        for (int palId = 0; palId < paletteCount; palId++)
        {
            var pal = new Palette(paletteSize, color0Transparent);
            Add(pal);
            int initColId = 0;
            if (color0Transparent)
            {
                initColId++;
                cumulativeOffset++;
            }
            for (int colId = initColId; colId < paletteSize; colId++)
            {
                pal.Add(PaletteUtil.To32BitColor(palette[cumulativeOffset++]));
            }
        }
    }
}
