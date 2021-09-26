using Core.Randomization;
using Core.Services;
using RanseiWpf.Dialogs;
using RanseiWpf.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class ModListItem : ViewModelBase
    {
        private readonly ModSelectionViewModel ParentVm;
        private readonly IModService ModService;
        private readonly IDialogService DialogService;

        public ModListItem(ModSelectionViewModel parentVm, ModInfo info, IModService modService, IDialogService dialogService)
        {
            ParentVm = parentVm;
            ModService = modService;
            DialogService = dialogService;
            Info = info;

            PatchRomCommand = new RelayCommand(PatchRom);
            ExportModCommand = new RelayCommand(ExportMod);
            EditModInfoCommand = new RelayCommand(EditModInfo);
            CreateModBasedOnCommand = new RelayCommand(CreateModBasedOn);
            RandomizeCommand = new RelayCommand(Randomize);
            DeleteModCommand = new RelayCommand(DeleteMod);
        }

        private ModInfo _info;
        public ModInfo Info
        {
            get => _info;
            private set => RaiseAndSetIfChanged(ref _info, value);
        }


        public ICommand PatchRomCommand { get; }
        public ICommand ExportModCommand { get; }
        public ICommand EditModInfoCommand { get; }
        public ICommand CreateModBasedOnCommand { get; }
        public ICommand RandomizeCommand { get; }
        public ICommand DeleteModCommand { get; }

        #region Mod Specific Command Implementations

        private void PatchRom()
        {
            var info = Info;
            if (DialogService.CommitToRom(info, out string targetRom))
            {
                try
                {
                    ModService.Commit(Info, targetRom);
                }
                catch (Exception e)
                {
                    DialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error Writing To Rom",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }
                
            }
        }
        private void ExportMod()
        {
            if (DialogService.ExportMod(Info, out string folder))
            {
                try
                {
                    ModService.Export(Info, folder);
                }
                catch (Exception e)
                {
                    DialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error Exporting Mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }
            }
        }
        private void EditModInfo()
        {
            if (DialogService.EditModInfo(Info))
            {
                ModService.Update(Info);
                ParentVm.RefreshModItems();
            }
        }
        private void CreateModBasedOn()
        {
            var baseInfo = Info;
            if (DialogService.CreateModBasedOn(baseInfo, out ModInfo newModInfo))
            {
                ModInfo newMod;
                try
                {
                    newMod = ModService.CreateBasedOn(Info, newModInfo.Name, newModInfo.Version, newModInfo.Author);
                }
                catch (Exception e)
                {
                    DialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error creating mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }

                ParentVm.AddListItem(newMod);
            }
        }

        private async void Randomize()
        {
            IRandomizer randomizer = new SimpleRandomizer();
            if (DialogService.Randomize(randomizer))
            {
                var dialog = new LoadingDialog("Randomizing...");
                dialog.Owner = App.Current.MainWindow;
                dialog.Show();
                await Task.Run(() => randomizer.Apply(new DataService(Info)));
                dialog.Close();
            }
        }

        private void DeleteMod()
        {
            if (DialogService.ConfirmDelete(Info))
            {
                ModService.Delete(Info);
                ParentVm.ModItems.Remove(this);
            }
        }
        #endregion
    }

    public class ModSelectionViewModel : ViewModelBase
    {
        private readonly IModService ModService;
        private readonly IDialogService DialogService;

        public ModSelectionViewModel(IWpfAppServices services)
        {
            ModService = services.CoreServices.ModService;
            DialogService = services.DialogService;

            RefreshModItems();

            CreateModCommand = new RelayCommand(CreateMod);
            ImportModCommand = new RelayCommand(ImportMod);

            ModItemClicked = new RelayCommand<ModInfo>(mi =>
            {
                ModSelected?.Invoke(mi);
            });
        }

        public ObservableCollection<ModListItem> ModItems { get; } = new ObservableCollection<ModListItem>();

        public ICommand ModItemClicked { get; }

        public event Action<ModInfo> ModSelected;

        public void RefreshModItems()
        {
            ModItems.Clear();
            foreach (var mi in ModService.GetAllModInfo())
            {
                AddListItem(mi);
            }
        }

        public void AddListItem(ModInfo info) => ModItems.Add(new ModListItem(this, info, ModService, DialogService));
        public ICommand CreateModCommand { get; }
        public ICommand ImportModCommand { get; }


        private void CreateMod()
        {
            if (DialogService.CreateMod(out ModInfo modInfo, out string romPath))
            {
                ModInfo newMod;
                try
                {
                    newMod = ModService.Create(romPath, modInfo.Name, modInfo.Version, modInfo.Author);
                }
                catch (Exception e)
                {
                    DialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error creating mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }

                AddListItem(newMod);
            }
        }
        private void ImportMod()
        {
            if (DialogService.ImportMod(out string file))
            {
                try
                {
                    ModInfo info = ModService.Import(file);
                    AddListItem(info);
                }
                catch (Exception e)
                {
                    DialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error importing mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }
            }
        }
    }
}
