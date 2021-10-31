using RanseiLink.Core.Services;
using RanseiLink.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels
{
    public class ModSelectionViewModel : ViewModelBase
    {
        private readonly IModService _modService;
        private readonly IDialogService _dialogService;
        private readonly DataServiceFactory _dataServiceFactory;

        public ModSelectionViewModel(IServiceContainer container)
        {
            _modService = container.Resolve<IModService>();
            _dialogService = container.Resolve<IDialogService>();
            _dataServiceFactory = container.Resolve<DataServiceFactory>();


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
                ModItems.Add(new ModListItemViewModel(this, mi, _modService, _dialogService, _dataServiceFactory));
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
                    _dialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error creating mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
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
                    _dialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Title = "Error importing mod",
                        Message = e.Message,
                        Icon = System.Windows.MessageBoxImage.Error
                    });
                    return;
                }
                RefreshModItems();
            }
        }
    }
}
