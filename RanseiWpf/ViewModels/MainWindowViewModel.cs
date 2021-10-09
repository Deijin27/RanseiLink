using Core.Services;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
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
                    if (currentVm is ISaveable saveable)
                    {
                        saveable.Save();
                    }
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
    }
}
