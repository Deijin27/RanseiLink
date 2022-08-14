namespace RanseiLink.Core.Graphics
{
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
            switch (format)
            {
                case TexFormat.None: return 0;
                case TexFormat.A3I5: return 8;
                case TexFormat.Pltt4: return 2;
                case TexFormat.Pltt16: return 4;
                case TexFormat.Pltt256: return 8;
                case TexFormat.Comp4x4: return 2;
                case TexFormat.A5I3: return 8;
                case TexFormat.Direct: return 16;
                default: throw new System.Exception("Invalid TexFormat");
            }
        }

        public static int PaletteSize(this TexFormat format)
        {
            switch (format)
            {
                case TexFormat.None: return 0;
                case TexFormat.A3I5: return 32;
                case TexFormat.Pltt4: return 4;
                case TexFormat.Pltt16: return 16;
                case TexFormat.Pltt256: return 256;
                case TexFormat.Comp4x4: return 4;
                case TexFormat.A5I3: return 8;
                case TexFormat.Direct: return 0;
                default: throw new System.Exception("Invalid TexFormat");
            }
        }
    }

}