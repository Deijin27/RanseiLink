using RanseiWpf.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IWpfAppServices Services;
        public MainWindowViewModel(IWpfAppServices services)
        {
            Services = services;
            ModSelectionVm = new ModSelectionViewModel(services);
            CurrentVm = ModSelectionVm;

            ModSelectionVm.ModSelected += mi =>
            {
                var mevm = new MainEditorViewModel(services, mi);
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
