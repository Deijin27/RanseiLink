namespace RanseiLink.Core.Graphics;

public enum TexFormat
{
    None,
    A3I5,
    Pltt4,
    Pltt16,
    Pltt256,
    Comp4x4,
    A5I3,
    Direct
}

public static class TexFormatExtensions
{
    public static int BitsPerPixel(this TexFormat format)
    {
        return format switch
        {
            TexFormat.None => 0,
            TexFormat.A3I5 => 8,
            TexFormat.Pltt4 => 2,
            TexFormat.Pltt16 => 4,
            TexFormat.Pltt256 => 8,
            TexFormat.Comp4x4 => 2,
            TexFormat.A5I3 => 8,
            TexFormat.Direct => 16,
            _ => throw new System.Exception("Invalid TexFormat"),
        };
    }

    public static int PaletteSize(this TexFormat format)
    {
        return format switch
        {
            TexFormat.None => 0,
            TexFormat.A3I5 => 32,
            TexFormat.Pltt4 => 4,
            TexFormat.Pltt16 => 16,
            TexFormat.Pltt256 => 256,
            TexFormat.Comp4x4 => 4,
            TexFormat.A5I3 => 8,
            TexFormat.Direct => 0,
            _ => throw new System.Exception("Invalid TexFormat"),
        };
    }
}