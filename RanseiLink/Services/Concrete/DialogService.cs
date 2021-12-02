using RanseiLink.Core.Services;
using RanseiLink.ViewModels;
using System.Windows;

namespace RanseiLink.Services.Concrete;

internal class DialogService : IDialogService
{
    private readonly ISettingsService Settings;
    public DialogService(ISettingsService settings)
    {
        Settings = settings;
    }

    public MessageBoxResult ShowMessageBox(MessageBoxArgs options)
    {
        MessageBox.Show(options.Message, options.Title);
        return MessageBoxResult.Ok;
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
        var vm = new ModCreationViewModel(this, Settings.RecentLoadRom);

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
            Settings.RecentLoadRom = vm.File;
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
        var vm = new ModExportViewModel(info, Settings.RecentExportModFolder);

        var dialog = new Dialogs.ModExportDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            folder = vm.Folder;
            Settings.RecentExportModFolder = vm.Folder;
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

    public bool CommitToRom(ModInfo info, out string romPath)
    {
        var vm = new ModCommitViewModel(this, info, Settings.RecentCommitRom);

        var dialog = new Dialogs.ModCommitDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = vm
        };

        bool? proceed = dialog.ShowDialog();

        if (proceed == true)
        {
            romPath = vm.File;
            Settings.RecentCommitRom = vm.File;
            return true;
        }
        else
        {
            romPath = null;
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
}
