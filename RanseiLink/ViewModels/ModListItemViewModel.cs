using RanseiLink.Core.Randomization;
using RanseiLink.Core.Services;
using RanseiLink.Dialogs;
using RanseiLink.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiLink.ViewModels
{
    public class ModListItemViewModel : ViewModelBase
    {
        private readonly ModSelectionViewModel _parentVm;
        private readonly IModService _modService;
        private readonly IDialogService _dialogService;
        private readonly DataServiceFactory _dataServiceFactory;

        public ModListItemViewModel(ModSelectionViewModel parentVm, ModInfo mod, IModService modService, IDialogService dialogService, DataServiceFactory dataServiceFactory)
        {
            _parentVm = parentVm;
            _modService = modService;
            _dialogService = dialogService;
            _dataServiceFactory = dataServiceFactory;
            Mod = mod;

            PatchRomCommand = new RelayCommand(() => PatchRom(Mod));
            ExportModCommand = new RelayCommand(() => ExportMod(Mod));
            EditModInfoCommand = new RelayCommand(() => EditModInfo(Mod));
            CreateModBasedOnCommand = new RelayCommand(() => CreateModBasedOn(Mod));
            RandomizeCommand = new RelayCommand(() => Randomize(Mod));
            DeleteModCommand = new RelayCommand(() => DeleteMod(Mod));
        }

        private ModInfo _mod;
        public ModInfo Mod
        {
            get => _mod;
            private set => RaiseAndSetIfChanged(ref _mod, value);
        }


        public ICommand PatchRomCommand { get; }
        public ICommand ExportModCommand { get; }
        public ICommand EditModInfoCommand { get; }
        public ICommand CreateModBasedOnCommand { get; }
        public ICommand RandomizeCommand { get; }
        public ICommand DeleteModCommand { get; }

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
                    _dialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error Writing To Rom",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
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
                    _dialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error Exporting Mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
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
                    _dialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error creating mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }

                _parentVm.RefreshModItems();
            }
        }

        private async void Randomize(ModInfo mod)
        {
            IRandomizer randomizer = new SimpleRandomizer();
            if (_dialogService.Randomize(randomizer))
            {
                var dialog = new LoadingDialog("Randomizing...");
                dialog.Owner = App.Current.MainWindow;
                dialog.Show();
                await Task.Run(() => randomizer.Apply(_dataServiceFactory(mod)));
                dialog.Close();
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
        #endregion
    }
}
