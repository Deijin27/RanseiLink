#nullable enable
using FluentResults;
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;

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
        var info = AnimationTypeInfoResource.Get(type);
        var (animLinkFile, bgLinkFile) = manager.GetDataFile(info, id);

        string? outputAnimLinkFile = null;
        string? outputBgLinkFile = null;
        Result<ImportResult> importResult;

        if (animLinkFile != null)
        {
            var animationXml = await dialogService.ShowOpenSingleFileDialog(new(
                "Choose an animation file to import", 
                new FileDialogFilter("Nitro Cell Animation", ".xml")
                ));
            if (animationXml == null)
            {
                return false;
            }
            
            outputAnimLinkFile = Path.GetTempFileName();
            if (bgLinkFile == null)
            {
                // there is no background associated with this animation
                importResult = CellAnimationSerialiser.ImportAnimation(
                    new CellImageSettings(info.Prt),
                    animLinkFile: animLinkFile.File,
                    animationXml: animationXml,
                    width: info.Width,
                    height: info.Height,
                    outputAnimLinkFile: outputAnimLinkFile
                    );
            }
            else
            {
                outputBgLinkFile = Path.GetTempFileName();
                // this has both background and animation
                importResult = CellAnimationSerialiser.ImportAnimAndBackground(
                    info: info,
                    new CellImageSettings(info.Prt),
                    animationXml: animationXml,
                    animLinkFile: animLinkFile.File,
                    outputAnimLinkFile: outputAnimLinkFile,
                    bgLinkFile: bgLinkFile.File,
                    outputBgLinkFile: outputBgLinkFile
                    );
            }
        }
        else if (bgLinkFile != null)
        {
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
            outputBgLinkFile = Path.GetTempFileName();
            importResult = CellAnimationSerialiser.ImportBackground(
                info: info,
                bgImage: backgroundImg,
                bgLinkFile: bgLinkFile.File,
                outputBgLinkFile: outputBgLinkFile
                ).ToResult();
        }
        else
        {
            throw new Exception("Both animation and background link files were null");
        }

        if (importResult.IsFailed)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Error Importing Animation", importResult.ToString(), MessageBoxType.Error));
            return false;
        }

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
