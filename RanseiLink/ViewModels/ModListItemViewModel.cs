using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate ModListItemViewModel ModListItemViewModelFactory(ModSelectionViewModel parent, ModInfo mod);

public class ModListItemViewModel : ViewModelBase
{
    private readonly ModSelectionViewModel _parentVm;
    private readonly IModManager _modService;
    private readonly IDialogService _dialogService;
    private readonly IServiceContainer _container;

    public ModListItemViewModel(ModSelectionViewModel parent, ModInfo mod, IServiceContainer container)
    {
        _container = container;
        _parentVm = parent;
        _modService = container.Resolve<IModManager>();
        _dialogService = container.Resolve<IDialogService>();
        Mod = mod;
        PluginItems = container.Resolve<IPluginLoader>().LoadPlugins(out var _);

        PatchRomCommand = new RelayCommand(() => PatchRom(Mod));
        ExportModCommand = new RelayCommand(() => ExportMod(Mod));
        EditModInfoCommand = new RelayCommand(() => EditModInfo(Mod));
        CreateModBasedOnCommand = new RelayCommand(() => CreateModBasedOn(Mod));
        DeleteModCommand = new RelayCommand(() => DeleteMod(Mod));
        RunPluginCommand = new RelayCommand<PluginInfo>(parameter => RunPlugin(Mod, parameter));
    }

    public IReadOnlyCollection<PluginInfo> PluginItems { get; }
    public ModInfo Mod { get; }
    public ICommand PatchRomCommand { get; }
    public ICommand ExportModCommand { get; }
    public ICommand EditModInfoCommand { get; }
    public ICommand CreateModBasedOnCommand { get; }
    public ICommand RandomizeCommand { get; }
    public ICommand DeleteModCommand { get; }
    public ICommand RunPluginCommand { get; }

    #region Mod Specific Command Implementations

    private void PatchRom(ModInfo mod)
    {
        if (!_dialogService.CommitToRom(Mod, out string romPath, out var patchOpt))
        {
            return;
        }

        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            try
            {
                _modService.Commit(mod, romPath, patchOpt, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: "Error Writing To Rom",
                message: error.Message,
                type: MessageBoxType.Error
            ));
        }
    }
    private void ExportMod(ModInfo mod)
    {
        if (!_dialogService.ExportMod(mod, out string folder))
        {
            return;
        }
        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Exporting mod..."));
            try
            {
                _modService.Export(mod, folder);
            }
            catch (Exception e)
            {
                error = e;
                return;
            }
            progress.Report(new ProgressInfo("Export Complete!", 100));
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Exporting Mod",
                    message: error.Message,
                    type: MessageBoxType.Error
                ));
        }
        
    }
    private void EditModInfo(ModInfo mod)
    {
        if (!_dialogService.EditModInfo(mod))
        {
            return;
        }
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Editing mod info..."));
            _modService.Update(mod);
            progress.Report(new ProgressInfo("Updating mod list...", 50));
            _parentVm.RefreshModItems();
            progress.Report(new ProgressInfo("Edit Complete!", 100));
        });
    }
    private void CreateModBasedOn(ModInfo mod)
    {
        if (!_dialogService.CreateModBasedOn(mod, out ModInfo newModInfo))
        {
            return;
        }
        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Creating mod..."));
            ModInfo newMod;
            try
            {
                newMod = _modService.CreateBasedOn(mod, newModInfo.Name, newModInfo.Version, newModInfo.Author);
            }
            catch (Exception e)
            {
                error = e;
                return;
            }

            progress.Report(new ProgressInfo("Updating mod list...", 60));
            _parentVm.RefreshModItems();
            progress.Report(new ProgressInfo("Mod Creating Complete!", 100));
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Creating Mod",
                    message: error.Message,
                    type: MessageBoxType.Error
                ));
        }
    }

    private void DeleteMod(ModInfo mod)
    {
        if (!_dialogService.ConfirmDelete(mod))
        {
            return;
        }
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Deleting mod..."));
            _modService.Delete(mod);
            progress.Report(new ProgressInfo("Updating mod list", 90));
            Application.Current.Dispatcher.Invoke(() => _parentVm.ModItems.Remove(this));
            progress.Report(new ProgressInfo("Mod Deleted!", 100));
        });
    }

    private void RunPlugin(ModInfo mod, PluginInfo chosen)
    {
        try
        {
            chosen.Plugin.Run(new PluginContext(_container, mod));
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.ToString(),
                type: MessageBoxType.Error
                ));
        }
    }
    #endregion
}
