using RanseiLink.Core.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, ISaveable
    {
        public MainWindowViewModel(IServiceContainer container)
        {
            ModSelectionVm = new ModSelectionViewModel(container);
            CurrentVm = ModSelectionVm;

            ModSelectionVm.ModSelected += mi =>
            {
                var mevm = new MainEditorViewModel(container, mi);
                CurrentVm = mevm;
                BackButtonVisible = true;
            };

            BackButtonCommand = new RelayCommand(() =>
            {
                CurrentVm = ModSelectionVm;
                BackButtonVisible = false;
            });
        }


        private readonly ModSelectionViewModel ModSelectionVm;

        private object currentVm;
        public object CurrentVm
        {
            get => currentVm;
            set
            {
                if (currentVm != value)
                {
                    Save();
                    currentVm = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _backButtonVisible;
        public bool BackButtonVisible
        {
            get => _backButtonVisible;
            set => RaiseAndSetIfChanged(ref _backButtonVisible, value);
        }

        public ICommand BackButtonCommand { get; }

        public void OnShutdown()
        {
            Save();
        }

        public void Save()
        {
            if (currentVm is ISaveable saveable)
            {
                saveable.Save();
            }
        }
    }
}
