using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Windows.ValueConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.Windows.ViewModels;

public delegate IModListItemViewModel ModListItemViewModelFactory(IModSelectionViewModel parent, ModInfo mod);

public interface IModListItemViewModel
{
    void UpdateBanner();
}
public class ModListItemViewModel : ViewModelBase, IModListItemViewModel
{
    private readonly IModSelectionViewModel _parentVm;
    private readonly IModManager _modService;
    private readonly IDialogService _dialogService;
    private readonly ISettingService _settingService;
    private readonly IModServiceGetterFactory _modKernelFactory;
    private readonly IFileDropHandlerFactory _fdhFactory;
    private readonly IFolderDropHandler _folderDropHandler;
    private readonly IModPatchingService _modPatcher;

    public ModListItemViewModel(
        IModSelectionViewModel parent,
        ModInfo mod,
        IModManager modManager,
        IModPatchingService modPatcher,
        IDialogService dialogService,
        ISettingService settingService,
        IPluginLoader pluginLoader,
        IModServiceGetterFactory modKernelFactory,
        IFileDropHandlerFactory fdhFactory,
        IFolderDropHandler folderDropHandler)
    {
        _settingService = settingService;
        _modKernelFactory = modKernelFactory;
        _fdhFactory = fdhFactory;
        _folderDropHandler = folderDropHandler;
        _parentVm = parent;
        _modService = modManager;
        _dialogService = dialogService;
        _modPatcher = modPatcher;
        Mod = mod;
        PluginItems = pluginLoader.LoadPlugins(out var _);

        UpdateBanner();

        PatchRomCommand = new RelayCommand(() => PatchRom(Mod));
        ExportModCommand = new RelayCommand(() => ExportMod(Mod));
        EditModInfoCommand = new RelayCommand(() => EditModInfo(Mod));
        CreateModBasedOnCommand = new RelayCommand(() => CreateModBasedOn(Mod));
        DeleteModCommand = new RelayCommand(() => DeleteMod(Mod));
        RunPluginCommand = new RelayCommand<PluginInfo>(parameter => RunPlugin(Mod, parameter));
        ShowInExplorerCommand = new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start("explorer.exe", Mod.FolderPath);
        });
    }

    public void UpdateBanner()
    {
        Banner = PathToImageSourceConverter.TryConvert(Path.Combine(Mod.FolderPath, Core.Services.Constants.BannerImageFile));
    }

    private ImageSource _banner;
    public ImageSource Banner
    {
        get => _banner;
        set => RaiseAndSetIfChanged(ref _banner, value);
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
    public ICommand ShowInExplorerCommand { get; }

    #region Mod Specific Command Implementations

    private void PatchRom(ModInfo mod)
    {
        var vm = new ModCommitViewModel(_dialogService, _settingService, mod, _fdhFactory);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }

        var canPatch = _modPatcher.CanPatch(mod, vm.File, vm.PatchOpt);
        if (canPatch.IsFailed)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unable to patch", canPatch.ToString()));
            return;
        }

        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            try
            {
                _modPatcher.Patch(mod, vm.File, vm.PatchOpt, progress);
            }
            catch (Exception e)
            {
                error = e;
            }
        });

        if (error != null)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Writing To Rom",
                message: error.ToString(),
                type: MessageBoxType.Error
            ));
        }
    }
    private void ExportMod(ModInfo mod)
    {
        var vm = new ModExportViewModel(_dialogService, _settingService, mod, _folderDropHandler);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception error = null;
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Exporting mod..."));
            try
            {
                _modService.Export(mod, vm.Folder);
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
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    title: "Error Exporting Mod",
                    message: error.ToString(),
                    type: MessageBoxType.Error
                ));
        }

    }
    private void EditModInfo(ModInfo mod)
    {
        var vm = new ModEditInfoViewModel(mod);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Editing mod info..."));
            _modService.Update(vm.ModInfo);
            progress.Report(new ProgressInfo("Updating mod list...", 50));
            _parentVm.RefreshModItems();
            progress.Report(new ProgressInfo("Edit Complete!", 100));
        });
    }
    private void CreateModBasedOn(ModInfo mod)
    {
        var vm = new ModCreateBasedOnViewModel(mod);
        if (!_dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        var newModInfo = vm.ModInfo;
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
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    title: "Error Creating Mod",
                    message: error.ToString(),
                    type: MessageBoxType.Error
                ));
        }
    }

    private void DeleteMod(ModInfo mod)
    {
        var vm = new ModDeleteViewModel(mod);
        if (!_dialogService.ShowDialogWithResult(vm))
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
            using (var serviceGetter = _modKernelFactory.Create(mod))
            {
                chosen.Plugin.Run(new PluginContext(serviceGetter));
            }
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.ToString(),
                type: MessageBoxType.Error
                ));
        }
    }
    #endregion
}
