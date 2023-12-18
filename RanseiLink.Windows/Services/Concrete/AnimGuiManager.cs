#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Windows.Services.Concrete;
internal class AnimGuiManager(ICellAnimationManager _manager, IAsyncDialogService _dialogService, ISettingService _settingService, ModInfo _mod) : IAnimGuiManager
{
    public async Task<bool> Export(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        CellAnimationSerialiser.Format[] formats;
        CellAnimationSerialiser.Format format;
        if (info.ExportFormat == null)
        {
            formats = Enum.GetValues<CellAnimationSerialiser.Format>();
            format = CellAnimationSerialiser.Format.OneImagePerBank;
        }
        else
        {
            formats = [info.ExportFormat.Value];
            format = info.ExportFormat.Value;
        }

        var dialogVm = new AnimExportViewModel(_dialogService, _settingService, _mod, formats, format);
        if (!await _dialogService.ShowDialogWithResult(dialogVm))
        {
            return false;
        }

        _manager.Export(type, id, FileUtil.MakeUniquePath(Path.Combine(dialogVm.Folder, $"anim_{type}_{id}")), dialogVm.SelectedFormat);
        return true;
    }

    public async Task<bool> Import(AnimationTypeId type, int id)
    {
        var filters = new List<FileDialogFilter>();
        var current = _manager.GetDataFile(type, id);
        bool chooseBackgroundFile;
        if (File.Exists(current.AnimationLink.File)) // if there's no animation, e.g. for Castlemap aurora, we just ask for a background
        {
            chooseBackgroundFile = false;
            filters.Add(new FileDialogFilter("Nitro Cell Animation", ".xml"));
        }
        else
        {
            chooseBackgroundFile = true;
            filters.Add(new FileDialogFilter("Animation Background Image", ".png"));
        }

        var file = await _dialogService.ShowOpenSingleFileDialog(new("Choose a file to import", filters.ToArray()));

        if (file == null)
        {
            return false;
        }

        string? animation = null;
        string? background;
        if (chooseBackgroundFile)
        {
            background = file;
        }
        else
        {
            animation = file;
            background = Path.Combine(Path.GetDirectoryName(file)!, "background.png");
        }
        
        _manager.Import(type, id, animation, background);

        return true;
    }

    public bool IsOverriden(AnimationTypeId type, int id)
    {
        var (anim, bg) = _manager.GetDataFile(type, id);
        return anim.IsOverride || (bg != null && bg.IsOverride);
    }

    public async Task<bool> RevertToDefault(AnimationTypeId type, int id)
    {
        if (!IsOverriden(type, id))
        {
            return false;
        }
        var result = await _dialogService.ShowMessageBox(new(
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
        _manager.ClearOverride(type, id);
        return true;
    }
}
