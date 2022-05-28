using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System;
using System.IO;

namespace RanseiLink.Core.Services.DefaultPopulaters
{
    public class MiscDefaultPopulater : IGraphicTypeDefaultPopulater
    {
        public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
        {
            if (!(gInfo is MiscConstants miscInfo))
            {
                return;
            }

            foreach (var item in miscInfo.Items)
            {
                switch (item)
                {
                    case NcerMiscItem ncer:
                        ProcessNcer(defaultDataFolder, ncer);
                        break;
                    case NscrMiscItem nscr:
                        ProcessNscr(defaultDataFolder, nscr);
                        break;
                }
            }
        }

        private void ProcessNcer(string defaultDataFolder, NcerMiscItem item)
        {
            string pngFile = Path.Combine(defaultDataFolder, item.PngFile);

            LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), Path.Combine(defaultDataFolder, item.LinkFolder), true, 4);
            var ncer = NCER.Load(Path.Combine(defaultDataFolder, item.Ncer));
            var ncgrPath = Path.Combine(defaultDataFolder, item.Ncgr);
            if (new FileInfo(ncgrPath).Length == 0)
            {
                ncgrPath = Path.Combine(defaultDataFolder, item.NcgrAlt);
            }
            var ncgr = NCGR.Load(ncgrPath);
            var nclr = NCLR.Load(Path.Combine(defaultDataFolder, item.Nclr));

            int width;
            int height;

            if (ncgr.Pixels.TilesPerColumn == -1 || ncgr.Pixels.TilesPerRow == -1)
            {
                width = -1;
                height = -1;
            }
            else
            {
                width = ncgr.Pixels.TilesPerRow * 8;
                height = ncgr.Pixels.TilesPerColumn * 8;
            }
            
            ImageUtil.SaveAsPng(
                file: pngFile,
                bank: ncer.CellBanks.Banks[0],
                blockSize: ncer.CellBanks.BlockSize,
                imageInfo: new ImageInfo(
                    pixels: ncgr.Pixels.Data, 
                    palette: RawPalette.To32bitColors(nclr.Palettes.Palette), 
                    width: width,
                    height: height
                    ),
                debug: false,
                tiled: ncgr.Pixels.IsTiled
                );
        }

        private static (int width, int height) GetSize(int fileSize, int bpp)
        {
            int num = fileSize * 8 / bpp;
            int num3;
            int num2;
            if (Math.Pow((int)Math.Sqrt(num), 2.0) == (double)num)
            {
                num3 = (num2 = (int)Math.Sqrt(num));
            }
            else
            {
                num3 = ((num < 256) ? num : 256);
                num2 = num / num3;
            }

            if (num2 == 0)
            {
                num2 = 1;
            }

            if (num3 == 0)
            {
                num3 = 1;
            }

            return (num3, num2);
        }

        private void ProcessNscr(string defaultDataFolder, NscrMiscItem item)
        {
            string pngFile = Path.Combine(defaultDataFolder, item.PngFile);

            LINK.Unpack(Path.Combine(defaultDataFolder, item.Link), Path.Combine(defaultDataFolder, item.LinkFolder), true, 4);

            var ncgrPath = Path.Combine(defaultDataFolder, item.Ncgr);
            if (new FileInfo(ncgrPath).Length == 0)
            {
                ncgrPath = Path.Combine(defaultDataFolder, item.NcgrAlt);
            }
            var ncgr = NCGR.Load(ncgrPath);
            var nclr = NCLR.Load(Path.Combine(defaultDataFolder, item.Nclr));

            ImageUtil.SaveAsPng(
                file: pngFile,
                imageInfo: new ImageInfo(
                    pixels: ncgr.Pixels.Data,
                    palette: RawPalette.To32bitColors(nclr.Palettes.Palette),
                    width: ncgr.Pixels.TilesPerRow * 8,
                    height: ncgr.Pixels.TilesPerColumn * 8
                    ),
                tiled: true
                );
        }
    }
}
