#nullable enable
using FluentResults;
using RanseiLink.Core;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Services.Concrete;
internal class AnimGuiManager(ICellAnimationManager manager, IAsyncDialogService dialogService, ISettingService settingService, ModInfo mod, IFolderDropHandler fdh) : IAnimGuiManager
{
    public async Task<bool> Export(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        RLAnimationFormat[] formats;
        RLAnimationFormat format;
        if (info.ExportFormat == null)
        {
            formats = Enum.GetValues<RLAnimationFormat>();
            format = RLAnimationFormat.OneImagePerCluster;
        }
        else
        {
            formats = [info.ExportFormat.Value];
            format = info.ExportFormat.Value;
        }

        var dialogVm = new AnimExportViewModel(dialogService, settingService, mod, formats, format, fdh);
        if (!await dialogService.ShowDialogWithResult(dialogVm))
        {
            return false;
        }

        manager.Export(info, id, FileUtil.MakeUniquePath(Path.Combine(dialogVm.Folder, $"anim_{type}_{id}")), dialogVm.SelectedFormat);
        return true;
    }

    public async Task<bool> Import(AnimationTypeId type, int id)
    {
        // Get DataFiles for type
        var info = AnimationTypeInfoResource.Get(type);
        var (animLinkFile, bgLinkFile) = manager.GetDataFile(info, id);

        var tempBg = FileUtil.GetTemporaryDirectory();

        string? outputAnimLinkFile = null;
        string? outputBgLinkFile = null;

        if (animLinkFile != null)
        {
            // -- Anim & Background -->

            // Choose an xml file to import

            var animationXml = await dialogService.ShowOpenSingleFileDialog(new(
                "Choose an animation file to import", 
                new FileDialogFilter("Nitro Cell Animation", ".xml")
                ));
            if (animationXml == null)
            {
                return false;
            }

            var tempAnim = FileUtil.GetTemporaryDirectory();
            var res = new RLAnimationResource(XDocument.Load(animationXml));

            outputAnimLinkFile = Path.GetTempFileName();
            if (bgLinkFile == null)
            {
                // -- Anim Only -->

                // Load Anim

                var dir = Path.GetDirectoryName(animationXml)!;
                LINK.Unpack(animLinkFile.File, tempAnim);
                var anim = G2DR.LoadAnimImgFromFolder(tempAnim);
                var valid = CellAnimationSerialiser.ValidateAndFixupAnim(res, anim.Nanr);
                if (valid.IsFailed)
                {
                    return false;
                }

                // Simplify palette, save to folder

                var (wasSimplified, simplifiedDir) = CellAnimationSerialiser.SimplifyPalette(
                    dir: dir,
                    res: res,
                    paletteSize: anim.Ncgr.Pixels.Format.PaletteSize()
                    );

                if (wasSimplified)
                {
                    // TODO: Are you happy with simplified palette?
                }

                // Import Folder
                var images = CellAnimationSerialiser.LoadImages(wasSimplified ? simplifiedDir : dir, res);
                var nanr = CellAnimationSerialiser.ImportAnimationXml(new(res, images), anim.Ncer, anim.Ncgr, anim.Nclr, info.Width, info.Height, new(info.Prt));
                G2DR.SaveAnimImgToFolder(tempAnim, nanr, anim.Ncer, anim.Ncgr, anim.Nclr, NcgrSlot.Infer);
                LINK.Pack(tempAnim, outputAnimLinkFile);

            }
            else
            {
                // -- Anim & Background -->

                // TODO: Load Background and Anim

                var dir = Path.GetDirectoryName(animationXml)!;
                LINK.Unpack(animLinkFile.File, tempAnim);
                var anim = G2DR.LoadAnimImgFromFolder(tempAnim);
                var valid = CellAnimationSerialiser.ValidateAndFixupAnim(res, anim.Nanr);
                if (valid.IsFailed)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(res.Background))
                {
                    // Background is required
                    return false;
                }

                var bgImage = Path.Combine(dir, res.Background);
                if (!File.Exists(bgImage))
                {
                    // File must exist
                    return false;
                }

                LINK.Unpack(bgLinkFile.File, tempBg);
                var bg = G2DR.LoadImgFromFolder(tempBg);
                

                // TODO: Simplify palette of both to folder

                var (wasSimplified, simplifiedDir) = CellAnimationSerialiser.SimplifyPalette(
                    dir: dir, 
                    res: res, 
                    paletteSize: anim.Ncgr.Pixels.Format.PaletteSize()
                    );
                var simglifiedBg = Path.Combine(simplifiedDir, res.Background);
                var wasBgSimplified = ImageSimplifier.SimplifyPalette(
                    imagePath: bgImage, 
                    maximumColors: bg.Ncgr.Pixels.Format.PaletteSize(), 
                    saveFile: simglifiedBg
                    );

                if (wasSimplified || wasBgSimplified)
                {
                    // TODO: Are you happy with simplified palette?
                }

                outputBgLinkFile = Path.GetTempFileName();

                // Import Folder
                var images = CellAnimationSerialiser.LoadImages(wasSimplified ? simplifiedDir : dir, res);
                var nanr = CellAnimationSerialiser.ImportAnimationXml(new(res, images), anim.Ncer, anim.Ncgr, anim.Nclr, info.Width, info.Height, new(info.Prt));
                G2DR.SaveAnimImgToFolder(tempAnim, nanr, anim.Ncer, anim.Ncgr, anim.Nclr, NcgrSlot.Infer);
                LINK.Pack(tempAnim, outputAnimLinkFile);

                var resl = CellAnimationSerialiser.ImportBackground(info, wasBgSimplified ? simglifiedBg : bgImage, bg.Ncgr, bg.Nclr);
                G2DR.SaveImgToFolder(tempBg, bg.Ncgr, bg.Nclr, NcgrSlot.Infer);
                LINK.Pack(tempBg, outputBgLinkFile);
            }
        }
        else if (bgLinkFile != null)
        {
            // -- Background Only -->

            // Choose png to import

            // if there's no animation, e.g. for Castlemap aurora,
            // we just ask for a background
            var backgroundImg = await dialogService.ShowOpenSingleFileDialog(new(
                "Choose a background file to import (this slot only supports backgrounds",
                new FileDialogFilter("Animation Background Image", ".png")
                ));
            if (backgroundImg == null)
            {
                return false;
            }

            var bgImage = backgroundImg;
            if (!File.Exists(bgImage))
            {
                // File must exist
                return false;
            }

            LINK.Unpack(bgLinkFile.File, tempBg);
            var bg = G2DR.LoadImgFromFolder(tempBg);

            var simglifiedBg = FileUtil.MakeUniquePath(Path.Combine(Path.GetDirectoryName(bgImage)!, Path.GetFileNameWithoutExtension(bgImage) + "-Simplified", Path.GetExtension(bgImage)));
            var wasBgSimplified = ImageSimplifier.SimplifyPalette(
                imagePath: bgImage,
                maximumColors: bg.Ncgr.Pixels.Format.PaletteSize(),
                saveFile: simglifiedBg
                );

            if (wasBgSimplified)
            {
                // TODO: Are you happy with simplified palette?
            }

            // Import simplified image

            outputBgLinkFile = Path.GetTempFileName();
            var resl = CellAnimationSerialiser.ImportBackground(info, wasBgSimplified ? simglifiedBg : bgImage, bg.Ncgr, bg.Nclr);
            G2DR.SaveImgToFolder(tempBg, bg.Ncgr, bg.Nclr, NcgrSlot.Infer);
            LINK.Pack(tempBg, outputBgLinkFile);
        }
        else
        {
            throw new Exception("Both animation and background link files were null");
        }

        // Set Override

        // Replace the current link files with the new ones
        manager.ClearOverride(info, id);
        manager.SetOverride(info, id, outputAnimLinkFile, outputBgLinkFile);

        // Clean up the temporary files
        if (File.Exists(outputAnimLinkFile))
        {
            File.Delete(outputAnimLinkFile);
        }
        if (File.Exists(outputBgLinkFile))
        {
            File.Delete(outputBgLinkFile);
        }

        return true;
    }

    public bool IsOverriden(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        var (anim, bg) = manager.GetDataFile(info, id);
        return (anim != null && anim.IsOverride) || (bg != null && bg.IsOverride);
    }

    public async Task<bool> RevertToDefault(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        if (!IsOverriden(type, id))
        {
            return false;
        }
        var result = await dialogService.ShowMessageBox(new(
            $"Revert animation to default?",
            "Confirm to permanently delete the internally stored animation and/or background which overrides the default",
            [
                new("Cancel", MessageBoxResult.Cancel),
                new("Yes, revert to default", MessageBoxResult.Yes)
            ]
            ));

        if (result != MessageBoxResult.Yes)
        {
            return false;
        }
        manager.ClearOverride(info, id);
        return true;
    }
}
