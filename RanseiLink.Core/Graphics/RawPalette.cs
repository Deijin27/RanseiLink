using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Graphics
{
    public static class RawPalette
    {
        public static Rgba32[] To32bitColors(IEnumerable<Rgb15> colors)
        {
            return colors.Select(color => new Rgba32((byte)(color.R * 8), (byte)(color.G * 8), (byte)(color.B * 8))).ToArray();
        }

        public static Rgb15[] From32bitColors(IEnumerable<Rgba32> colors)
        {
            return colors.Select(color => new Rgb15(color.R / 8, color.G / 8, color.B / 8)).ToArray();
        }

        public static Rgb15[] Decompress(byte[] data)
        {
            return data.ToUInt16Array().Select(i => Rgb15.From(i)).ToArray();
        }

        public static byte[] Compress(Rgb15[] colors)
        {
            return colors.Select(i => i.ToUInt16()).ToArray().ToByteArray();
        }

        public static void SaveAsPaintNetPalette(Rgba32[] colors, string file)
        {
            using (var sw = new StreamWriter(File.Create(file)))
            {
                foreach (var col in colors)
                {
                    sw.WriteLine($"{col.A.ToString("X").PadLeft(2, '0')}{col.R.ToString("X").PadLeft(2, '0')}{col.G.ToString("X").PadLeft(2, '0')}{col.B.ToString("X").PadLeft(2, '0')}");
                }
            } 
        }
    }
}