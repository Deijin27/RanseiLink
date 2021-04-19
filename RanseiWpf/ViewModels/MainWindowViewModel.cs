using Core.Enums;
using Core.Models;
using Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand PokemonViewCommand { get; set; }
        public ICommand WazaViewCommand { get; set; }

        public ICommand CommitChangesCommand { get; set; }

        object _currentView;
        public object CurrentView 
        {
            get => _currentView;
            set => RaiseAndSetIfChanged(ref _currentView, value);
        }
        SelectorViewModelBase<PokemonId, Pokemon, PokemonViewModel> PokemonVm;
        SelectorViewModelBase<MoveId, Move, WazaViewModel> WazaVm;
        DataService DataService;

        public MainWindowViewModel()
        {
            DataService = new DataService();

            PokemonVm = new SelectorViewModelBase<PokemonId, Pokemon, PokemonViewModel>(PokemonId.Pikachu, DataService);
            WazaVm = new SelectorViewModelBase<MoveId, Move, WazaViewModel>(MoveId.Thunderbolt, DataService);

            CurrentView = PokemonVm;

            PokemonViewCommand = new RelayCommand(o =>
            {
                CurrentView = PokemonVm;

            });
            WazaViewCommand = new RelayCommand(o =>
            {
                CurrentView = WazaVm;
            });

            CommitChangesCommand = new RelayCommand(o =>
            {
                PokemonVm.SaveAndClearCache();
                WazaVm.SaveAndClearCache();
            });
        }
    }
}
