using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.Settings;
using RanseiLink.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace RanseiLink.Services.Concrete;

internal class DialogService : IDialogService
{
    private readonly ISettingService _settingService;
    private readonly RecentLoadRomSetting _recentLoadRomSetting;
    private readonly RecentCommitRomSetting _recentCommitRomSetting;
    private readonly RecentExportModFolderSetting _recentExportModFolderSetting;
    private readonly PatchSpritesSetting _patchSpritesSetting;
    public DialogService(ISettingService settings)
    {
        _settingService = settings;
        _recentLoadRomSetting = settings.Get<RecentLoadRomSetting>();
        _recentCommitRomSetting = settings.Get<RecentCommitRomSetting>();
        _recentExportModFolderSetting = settings.Get<RecentExportModFolderSetting>();
        _patchSpritesSetting = settings.Get<PatchSpritesSetting>();
    }

    public Core.Services.MessageBoxResult ShowMessageBox(MessageBoxArgs options)
    {
        var dialog = new Dialogs.MessageBoxDialog(options)
        {
            Owner = Application.Current.MainWindow
        };
        dialog.ShowDialog();
        return dialog.Result;
    }

    public bool RequestFile(string title, string defaultExt, string filter, out string result)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = title,
            DefaultExt = defaultExt,
            Filter = filter,
            CheckFileExists = true,
            CheckPathExists = true,
        };

        // Show save file dialog box
        bool? proceed = dialog.ShowDialog();
        result = dialog.FileName;
        // Process save file dialog box results
        return proceed == true;
    }

    public bool RequestRomFile(out string result)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select a Rom",
            DefaultExt = ".nds",
            Filter = "Pokemon Conquest Rom (.nds)|*.nds",
            CheckFileExists = true,
            CheckPathExists = true,
        };

        // Show save file dialog box
        bool? proceed = dialog.ShowDialog();
        result = dialog.FileName;
        // Process save file dialog box results
        return proceed == true;
    }

    public bool RequestModFile(out string result)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select a Mod",
            DefaultExt = ".rlmod",
            Filter = "RanseiLink Mod (.rlmod)|*.rlmod",
            CheckFileExists = true,
            CheckPathExists = true,
        };

        // Show save file dialog box
        bool? proceed = dialog.ShowDialog();
        result = dialog.FileName;
        // Process save file dialog box results
        return proceed == true;
    }

    public bool RequestFolder(out string result)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Navigate to a folder and click Open",
            ValidateNames = false,
            CheckFileExists = false,
            CheckPathExists = true,
        };

        // Show save file dialog box
        bool? proceed = dialog.ShowDialog();
        result = dialog.FileName;
        // Process save file dialog box results
        if (proceed == true)
        {
            if (System.IO.Directory.Exists(result))
            {
                return true;
            }
            else if (System.IO.File.Exists(result))
            {
                result = System.IO.Path.GetDirectoryName(result);
                return true;
            }
        }
        return false;
    }

    public bool CreateMod(out ModInfo modInfo, out string romPath)
    {
        var vm = new ModCreationViewModel(this, _recentLoadRomSetting.Value);

        var dialog = new Dialogs.ModCreationDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            modInfo = vm.ModInfo;
            romPath = vm.File;
            _recentLoadRomSetting.Value = vm.File;
            _settingService.Save();
            return true;
        }
        else
        {
            modInfo = null;
            romPath = null;
            return false;
        }
    }

    public bool CreateModBasedOn(ModInfo baseMod, out ModInfo newModInfo)
    {
        var vm = new ModCreateBasedOnViewModel(baseMod);

        var dialog = new Dialogs.ModCreateBasedOnDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            newModInfo = vm.ModInfo;
            return true;
        }
        else
        {
            newModInfo = null;
            return false;
        }
    }

    public bool ExportMod(ModInfo info, out string folder)
    {
        var vm = new ModExportViewModel(info, _recentExportModFolderSetting.Value);

        var dialog = new Dialogs.ModExportDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            folder = vm.Folder;
            _recentExportModFolderSetting.Value = vm.Folder;
            _settingService.Save();
            return true;
        }
        else
        {
            folder = null;
            return false;
        }
    }

    public bool ImportMod(out string file)
    {
        var vm = new ModImportViewModel(this);

        var dialog = new Dialogs.ModImportDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            file = vm.File;
            return true;
        }
        else
        {
            file = null;
            return false;
        }
    }

    public bool EditModInfo(ModInfo info)
    {
        var vm = new ModEditInfoViewModel(new ModInfo() { Name = info.Name, Version = info.Version, Author = info.Author });

        var dialog = new Dialogs.ModEditInfoDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            info.Name = vm.Name;
            info.Author = vm.Author;
            info.Version = vm.Version;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CommitToRom(ModInfo info, out string romPath, out PatchOptions patchOptions)
    {
        var vm = new ModCommitViewModel(this, info, _recentCommitRomSetting.Value)
        {
            IncludeSprites = _patchSpritesSetting.Value
        };

        var dialog = new Dialogs.ModCommitDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm,
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            romPath = vm.File;
            _recentCommitRomSetting.Value = vm.File;
            _patchSpritesSetting.Value = vm.IncludeSprites;
            _settingService.Save();
            patchOptions = 0;

            if (vm.IncludeSprites)
            {
                patchOptions |= PatchOptions.IncludeSprites;
            }

            return true;
        }
        else
        {
            romPath = null;
            patchOptions = 0;
            return false;
        }
    }

    public bool ConfirmDelete(ModInfo info)
    {
        var vm = new ModDeleteViewModel(info);

        var dialog = new Dialogs.ModDeleteDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        return proceed == true;
    }

    public void ProgressDialog(Action<IProgress<ProgressInfo>> work, bool delayOnCompletion = true)
    {
        var progressWindow = new Dialogs.LoadingDialog
        {
            Owner = Application.Current.MainWindow
        };

        var progressReporter = new Progress<ProgressInfo>(info =>
        {
            if (info.StatusText != null)
            {
                progressWindow.HeaderTextBlock.Text = info.StatusText;
            }
            if (info.MaxProgress != null)
            {
                progressWindow.ProgressBar.Maximum = (int)info.MaxProgress;
            }
            if (info.Progress != null)
            {
                progressWindow.ProgressBar.Value = (int)info.Progress;
            }
            if (info.IsIndeterminate != null)
            {
                progressWindow.ProgressBar.IsIndeterminate = (bool)info.IsIndeterminate;
            }
        });

        progressWindow.Loaded += async (s, e) =>
        {
            await Task.Run(() => work(progressReporter));
            if (delayOnCompletion)
            {
                await Task.Delay(500);
            }
            progressWindow.Close();
        };

        progressWindow.ShowDialog();
    }

    public bool UpgradeMods(out string romPath)
    {
        var vm = new ModUpgradeViewModel(this, _recentLoadRomSetting.Value);

        var dialog = new Dialogs.ModUpgradeDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            romPath = vm.File;
            _recentLoadRomSetting.Value = vm.File;
            _settingService.Save();
            return true;
        }
        else
        {
            romPath = null;
            return false;
        }
    }

    public bool ModifyMapDimensions(ref ushort width, ref ushort height)
    {
        var dialog = new Dialogs.ModifyMapDimensionsDialog()
        {
            Owner = Application.Current.MainWindow
        };
        dialog.WidthNumberBox.Value = width;
        dialog.HeightNumberBox.Value = height;

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            width = (ushort)dialog.WidthNumberBox.Value;
            height = (ushort)dialog.HeightNumberBox.Value;
            return true;
        }
        return false;
    }

    public bool PopulateDefaultSprites(out string romPath)
    {
        var vm = new PopulateDefaultSpriteViewModel(this, _recentLoadRomSetting.Value);

        var dialog = new Dialogs.PopulateDefaultSpriteDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            romPath = vm.File;
            _recentLoadRomSetting.Value = vm.File;
            _settingService.Save();
            return true;
        }
        else
        {
            romPath = null;
            return false;
        }
    }

    public bool SimplfyPalette(int maxColors, string original, string simplified)
    {
        var vm = new PaletteSimplifierDialogViewModel(
            maxColors,
            original,
            simplified
        );

        var dialog = new Dialogs.SimplifyPaletteDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        return proceed == true;
    }
}
