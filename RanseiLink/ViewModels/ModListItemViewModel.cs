using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate ModListItemViewModel ModListItemViewModelFactory(ModSelectionViewModel parent, ModInfo mod);

public class ModListItemViewModel : ViewModelBase
{
    private readonly ModSelectionViewModel _parentVm;
    private readonly IModService _modService;
    private readonly IDialogService _dialogService;
    private readonly IServiceContainer _container;

    public ModListItemViewModel(ModSelectionViewModel parent, ModInfo mod, IServiceContainer container)
    {
        _container = container;
        _parentVm = parent;
        _modService = container.Resolve<IModService>();
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
        if (_dialogService.CommitToRom(mod, out string targetRom))
        {
            try
            {
                _modService.Commit(mod, targetRom);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Writing To Rom",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
                return;
            }

        }
    }
    private void ExportMod(ModInfo mod)
    {
        if (_dialogService.ExportMod(mod, out string folder))
        {
            try
            {
                _modService.Export(mod, folder);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Exporting Mod",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
                return;
            }
        }
    }
    private void EditModInfo(ModInfo mod)
    {
        if (_dialogService.EditModInfo(mod))
        {
            _modService.Update(mod);
            _parentVm.RefreshModItems();
        }
    }
    private void CreateModBasedOn(ModInfo mod)
    {
        if (_dialogService.CreateModBasedOn(mod, out ModInfo newModInfo))
        {
            ModInfo newMod;
            try
            {
                newMod = _modService.CreateBasedOn(mod, newModInfo.Name, newModInfo.Version, newModInfo.Author);
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Creating Mod",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
                return;
            }

            _parentVm.RefreshModItems();
        }
    }

    private void DeleteMod(ModInfo mod)
    {
        if (_dialogService.ConfirmDelete(mod))
        {
            _modService.Delete(mod);
            _parentVm.ModItems.Remove(this);
        }
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
                message: $"An error was encountered while running the plugin {chosen.Name} (v{chosen.Version} by {chosen.Author}). Details:\n\n" + e.Message,
                type: MessageBoxType.Error
                ));
        }
    }
    #endregion
}
