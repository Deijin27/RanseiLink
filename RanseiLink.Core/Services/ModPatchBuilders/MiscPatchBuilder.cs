using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.Concrete;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RanseiLink.Core.Services.ModPatchBuilders
{
    public class MiscPatchBuilder : IGraphicTypePatchBuilder
    {
        private readonly IOverrideSpriteProvider _overrideSpriteProvider;
        private readonly string _graphicsProviderFolder = Constants.DefaultDataProviderFolder;
        public MiscPatchBuilder(IOverrideSpriteProvider overrideSpriteProvider)
        {
            _overrideSpriteProvider = overrideSpriteProvider;
        }

        public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
        {
            if (!(gInfo is MiscConstants miscInfo))
            {
                return;
            }            

            foreach (var item in miscInfo.Items)
            {
                var spriteFile = _overrideSpriteProvider.GetSpriteFile(miscInfo.Type, item.Id);
                if (!spriteFile.IsOverride)
                {
                    continue;
                }

                switch (item)
                {
                    case NcerMiscItem ncer:
                        ProcessNcer(filesToPatch, ncer, spriteFile.File);
                        break;
                    case NscrMiscItem nscr:
                        ProcessNscr(filesToPatch, nscr, spriteFile.File);
                        break;
                }
            }
        }

        private void ProcessNcer(ConcurrentBag<FileToPatch> filesToPatch, NcerMiscItem item, string pngFile)
        {
            // load up provider data
            var ncer = NCER.Load(Path.Combine(_graphicsProviderFolder, item.Ncer));
            var ncgrPath = Path.Combine(_graphicsProviderFolder, item.Ncgr);
            if (new FileInfo(ncgrPath).Length == 0)
            {
                ncgrPath = Path.Combine(_graphicsProviderFolder, item.NcgrAlt);
            }
            var ncgr = NCGR.Load(ncgrPath);
            var nclr = NCLR.Load(Path.Combine(_graphicsProviderFolder, item.Nclr));

            // load up the png and replace provider data with new image data
            var imageInfo = ImageUtil.LoadPng(
                file: pngFile, 
                bank: ncer.CellBanks.Banks[0], 
                blockSize: ncer.CellBanks.BlockSize, 
                tiled: ncgr.Pixels.IsTiled
                );
            ncgr.Pixels.Data = imageInfo.Pixels;
            var newPalette = RawPalette.From32bitColors(imageInfo.Palette);
            if (newPalette.Length > item.PaletteCapacity)
            {
                // this should not be hit because it should be filtered out by the palette simplifier
                throw new Exception($"Invalid palette length for misc sprite {item.Id}");
            }
            newPalette.CopyTo(nclr.Palettes.Palette, 0);

            // save the modified provider files to temporary files ready for patching
            var tempNcgr = Path.GetTempFileName();
            var tempNclr = Path.GetTempFileName();
            ncgr.Save(tempNcgr);
            nclr.Save(tempNclr);

            filesToPatch.Add(new FileToPatch(ncgrPath, tempNcgr, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(item.Nclr, tempNclr, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }

        private void ProcessNscr(ConcurrentBag<FileToPatch> filesToPatch, NscrMiscItem item, string pngFile)
        {
            // load up provider data
            var ncgrPath = Path.Combine(_graphicsProviderFolder, item.Ncgr);
            if (new FileInfo(ncgrPath).Length == 0)
            {
                ncgrPath = Path.Combine(_graphicsProviderFolder, item.NcgrAlt);
            }
            var ncgr = NCGR.Load(ncgrPath);
            var nclr = NCLR.Load(Path.Combine(_graphicsProviderFolder, item.Nclr));

            // load up the png and replace provider data with new image data
            var imageInfo = ImageUtil.LoadPng(pngFile, ncgr.Pixels.IsTiled);
            ncgr.Pixels.Data = imageInfo.Pixels;
            var newPalette = RawPalette.From32bitColors(imageInfo.Palette);
            if (newPalette.Length > item.PaletteCapacity)
            {
                // this should not be hit because it should be filtered out by the palette simplifier
                throw new Exception($"Invalid palette length for misc sprite {item.Id}");
            }
            newPalette.CopyTo(nclr.Palettes.Palette, 0);

            // save the modified provider files to temporary files ready for patching
            var tempNcgr = Path.GetTempFileName();
            var tempNclr = Path.GetTempFileName();
            ncgr.Save(tempNcgr);
            nclr.Save(tempNclr);

            filesToPatch.Add(new FileToPatch(ncgrPath, tempNcgr, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(item.Nclr, tempNclr, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }
    }
}
