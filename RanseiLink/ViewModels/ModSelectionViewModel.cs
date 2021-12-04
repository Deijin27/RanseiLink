using RanseiLink.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ModSelectionViewModel : ViewModelBase
{
    private readonly IServiceContainer _container;
    private readonly IModService _modService;
    private readonly IDialogService _dialogService;

    public ModSelectionViewModel(IServiceContainer container)
    {
        _container = container;
        _modService = container.Resolve<IModService>();
        _dialogService = container.Resolve<IDialogService>();
        

        RefreshModItems();

        CreateModCommand = new RelayCommand(CreateMod);
        ImportModCommand = new RelayCommand(ImportMod);

        ModItemClicked = new RelayCommand<ModInfo>(mi =>
        {
            ModSelected?.Invoke(mi);
        });
    }

    public ObservableCollection<ModListItemViewModel> ModItems { get; } = new ObservableCollection<ModListItemViewModel>();

    public ICommand ModItemClicked { get; }

    public event Action<ModInfo> ModSelected;

    public void RefreshModItems()
    {
        ModItems.Clear();
        foreach (var mi in _modService.GetAllModInfo().OrderBy(i => i.Name))
        {
            ModItems.Add(new ModListItemViewModel(this, mi, _container));
        }
    }

    public ICommand CreateModCommand { get; }
    public ICommand ImportModCommand { get; }

    private void CreateMod()
    {
        if (_dialogService.CreateMod(out ModInfo modInfo, out string romPath))
        {
            ModInfo newMod;
            try
            {
                newMod = _modService.Create(romPath, modInfo.Name, modInfo.Version, modInfo.Author);
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

            RefreshModItems();
        }
    }
    private void ImportMod()
    {
        if (_dialogService.ImportMod(out string file))
        {
            try
            {
                _modService.Import(file);

            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    title: "Error Importing Mod",
                    message: e.Message,
                    type: MessageBoxType.Error
                ));
                return;
            }
            RefreshModItems();
        }
    }
}
