using Core.Enums;
using Core.Services;
using RanseiWpf.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand PokemonViewCommand { get; set; }
        public ICommand MoveViewCommand { get; set; }
        public ICommand AbilityViewCommand { get; set; }
        public ICommand WarriorSkillViewCommand { get; set; }

        public ICommand LoadRomCommand { get; set; }
        public ICommand CommitRomCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }


        object _currentView;
        public object CurrentView 
        {
            get => _currentView;
            set => RaiseAndSetIfChanged(ref _currentView, value);
        }
        public PokemonSelectorViewModel PokemonVm;
        public MoveSelectorViewModel MoveVm;
        public AbilitySelectorViewModel AbilityVm;
        public WarriorSkillSelectorViewModel WarriorSkillVm;

        readonly IDialogService DialogService;
        readonly IDataService DataService;

        public MainWindowViewModel()
        {
            DataService = new DataService();
            DialogService = new DialogService();
            Init();
        }

        public MainWindowViewModel(IDataService dataService, IDialogService dialogService)
        {
            DataService = dataService;
            DialogService = dialogService;
            Init();
        }

        void Init()
        {
            PokemonVm = new PokemonSelectorViewModel(PokemonId.Pikachu, DataService);
            MoveVm = new MoveSelectorViewModel(MoveId.Thunderbolt, DataService);
            AbilityVm = new AbilitySelectorViewModel(AbilityId.Static, DataService);
            WarriorSkillVm = new WarriorSkillSelectorViewModel(WarriorSkillId.Ambition, DataService);

            CurrentView = PokemonVm;

            PokemonViewCommand = new RelayCommand(o =>
            {
                CurrentView = PokemonVm;

            });
            MoveViewCommand = new RelayCommand(o =>
            {
                CurrentView = MoveVm;
            });
            AbilityViewCommand = new RelayCommand(o =>
            {
                CurrentView = AbilityVm;
            });
            WarriorSkillViewCommand = new RelayCommand(o =>
            {
                CurrentView = WarriorSkillVm;
            });

            SaveChangesCommand = new RelayCommand(o =>
            {
                SaveChanges();
            });

            LoadRomCommand = new RelayCommand(o => 
            {
                var proceed = DialogService.ShowMessageBox(new MessageBoxArgs() 
                { 
                    Title = "Load Rom",
                    Message = "Loading a new rom will overwrite any changes you have made. Proceed?",
                    Button = MessageBoxButton.OKCancel,
                    Icon = MessageBoxImage.Warning
                });
                if (proceed == MessageBoxResult.OK)
                {
                    if (DialogService.RequestRomFile("Chose a rom file to load", out string chosenFilePath))
                    {
                        DataService.LoadRom(chosenFilePath);

                        PokemonVm.ClearUnsavedChanges();
                        MoveVm.ClearUnsavedChanges();
                        AbilityVm.ClearUnsavedChanges();
                    }
                }
            });

            CommitRomCommand = new RelayCommand(o => 
            {
                var proceed = DialogService.ShowMessageBox(new MessageBoxArgs()
                {
                    Title = "Commit To Rom",
                    Message = "Unsaved changes will be saved to app data, then changes will be wrote to the rom file.\n" + 
                              "Writing to a rom file will overwrite data in the rom.\n" +
                              "Be sure to make an unchanged copy of your rom first.\n" + 
                              "Proceed?",
                    Button = MessageBoxButton.OKCancel,
                    Icon = MessageBoxImage.Warning
                });
                if (proceed == MessageBoxResult.OK)
                {
                    if (DialogService.RequestRomFile("Choose a rom file to commit to", out string chosenFilePath))
                    {
                        SaveChanges();
                        DataService.CommitToRom(chosenFilePath);
                    }
                }
            });
        }

        public void SaveChanges()
        {
            PokemonVm.SaveAndClearCache();
            MoveVm.SaveAndClearCache();
            AbilityVm.SaveAndClearCache();
            WarriorSkillVm.SaveAndClearCache();
        }
    }
}
