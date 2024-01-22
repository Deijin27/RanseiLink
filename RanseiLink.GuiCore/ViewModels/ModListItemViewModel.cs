using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.GuiCore.DragDrop;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;

namespace RanseiLink.GuiCore.ViewModels;

public delegate IModListItemViewModel ModListItemViewModelFactory(ModInfo mod, Func<List<string>> getKnownTags);

public interface IModListItemViewModel
{
    void UpdateBanner();
    event Action<IModListItemViewModel> RequestRemove;
    event Action RequestRefresh;
    ModInfo Mod { get; }
}
public class ModListItemViewModel : ViewModelBase, IModListItemViewModel
{
    private readonly IModManager _modService;
    private readonly IAsyncDialogService _dialogService;
    private readonly ISettingService _settingService;
    private readonly IModServiceGetterFactory _modKernelFactory;
    private readonly IFileDropHandlerFactory _fdhFactory;
    private readonly IFolderDropHandler _folderDropHandler;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly IModPatchingService _modPatcher;

    public ModListItemViewModel(
        IModManager modManager,
        IModPatchingService modPatcher,
        IAsyncDialogService dialogService,
        ISettingService settingService,
        IPluginLoader pluginLoader,
        IModServiceGetterFactory modKernelFactory,
        IFileDropHandlerFactory fdhFactory,
        IFolderDropHandler folderDropHandler,
        IPathToImageConverter pathToImageConverter)
    {
        _settingService = settingService;
        _modKernelFactory = modKernelFactory;
        _fdhFactory = fdhFactory;
        _folderDropHandler = folderDropHandler;
        _pathToImageConverter = pathToImageConverter;
        _modService = modManager;
        _dialogService = dialogService;
        _modPatcher = modPatcher;
        
        PluginItems = pluginLoader.LoadPlugins(out var _);

        PatchRomCommand = new RelayCommand(() => PatchRom(Mod));
        ExportModCommand = new RelayCommand(() => ExportMod(Mod));
        EditModInfoCommand = new RelayCommand(() => EditModInfo(Mod));
        CreateModBasedOnCommand = new RelayCommand(() => CreateModBasedOn(Mod));
        DeleteModCommand = new RelayCommand(() => DeleteMod(Mod));
        RunPluginCommand = new RelayCommand<PluginInfo>(async parameter => { if (parameter != null) await RunPlugin(Mod, parameter); });
        ShowInExplorerCommand = new RelayCommand(() =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Mod.FolderPath + "/",
                UseShellExecute = true
            });
        });
    }

    /// <summary>
    /// IMPORTANT: This must be called after construction or things go kaboom
    /// </summary>
    /// <param name="mod"></param>
    internal ModListItemViewModel Init(ModInfo mod, Func<List<string>> getKnownTags)
    {
        _getKnownTags = getKnownTags;
        Mod = mod;
        UpdateBanner();
        return this;
    }

    public void UpdateBanner()
    {
        Banner = _pathToImageConverter.TryConvert(Path.Combine(Mod.FolderPath, Core.Services.Constants.BannerImageFile));
    }

    private object? _banner;
    public object? Banner
    {
        get => _banner;
        set => RaiseAndSetIfChanged(ref _banner, value);
    }
    public IReadOnlyCollection<PluginInfo> PluginItems { get; }

    private Func<List<string>> _getKnownTags = null!;

    public ModInfo Mod { get; private set; } = null!;
    public ICommand PatchRomCommand { get; }
    public ICommand ExportModCommand { get; }
    public ICommand EditModInfoCommand { get; }
    public ICommand CreateModBasedOnCommand { get; }
    public ICommand DeleteModCommand { get; }
    public ICommand RunPluginCommand { get; }
    public ICommand ShowInExplorerCommand { get; }
    public event Action<IModListItemViewModel>? RequestRemove;
    public event Action? RequestRefresh;

    #region Mod Specific Command Implementations

    private async Task PatchRom(ModInfo mod)
    {
        var vm = new ModCommitViewModel(_dialogService, _settingService, mod, _fdhFactory);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }

        var canPatch = _modPatcher.CanPatch(mod, vm.File, vm.PatchOpt);
        if (canPatch.IsFailed)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unable to patch", canPatch.ToString()));
            return;
        }

        Exception? error = null;
        await _dialogService.ProgressDialog(progress =>
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
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Error Writing To Rom",
                message: error.ToString(),
                type: MessageBoxType.Error
            ));
        }
    }
    private async Task ExportMod(ModInfo mod)
    {
        var vm = new ModExportViewModel(_dialogService, _settingService, mod, _folderDropHandler);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception? error = null;
        await _dialogService.ProgressDialog(progress =>
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
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    title: "Error Exporting Mod",
                    message: error.ToString(),
                    type: MessageBoxType.Error
                ));
        }

    }
    private async Task EditModInfo(ModInfo mod)
    {
        var vm = new ModEditInfoViewModel(mod, _getKnownTags());
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Editing mod info..."));
            _modService.Update(vm.ModInfo);
            progress.Report(new ProgressInfo("Updating mod list...", 50));
            RequestRefresh?.Invoke();
            progress.Report(new ProgressInfo("Edit Complete!", 100));
        });
    }
    private async Task CreateModBasedOn(ModInfo mod)
    {
        var vm = new ModCreateBasedOnViewModel(mod, _getKnownTags());
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        Exception? error = null;
        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Creating mod..."));
            ModInfo newMod;
            try
            {
                newMod = _modService.CreateBasedOn(mod, vm.Metadata);
            }
            catch (Exception e)
            {
                error = e;
                return;
            }

            progress.Report(new ProgressInfo("Updating mod list...", 60));
            RequestRefresh?.Invoke();
            progress.Report(new ProgressInfo("Mod Creating Complete!", 100));
        });

        if (error != null)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    title: "Error Creating Mod",
                    message: error.ToString(),
                    type: MessageBoxType.Error
                ));
        }
    }

    private async Task DeleteMod(ModInfo mod)
    {
        var vm = new ModDeleteViewModel(mod);
        if (!await _dialogService.ShowDialogWithResult(vm))
        {
            return;
        }
        await _dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo("Deleting mod..."));
            _modService.Delete(mod);
            progress.Report(new ProgressInfo("Updating mod list", 90));
            RequestRemove?.Invoke(this);
            progress.Report(new ProgressInfo("Mod Deleted!", 100));
        });
    }

    private async Task RunPlugin(ModInfo mod, PluginInfo chosen)
    {
        try
        {
            using (var serviceGetter = _modKernelFactory.Create(mod))
            {
                await chosen.Plugin.Run(new PluginContext(serviceGetter));
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: $"Error running {chosen.Name}",
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.ToString(),
                type: MessageBoxType.Error
                ));
        }
    }
    #endregion
}
