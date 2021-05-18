using Core.Enums;
using RanseiWpf.ViewModels;
using Tests.Mocks;
using Xunit;
using System.Collections.Generic;
using Core.Models.Interfaces;
using Core.Models;

namespace Tests.ViewModelTests.MainWindowViewModelTests
{
    public class LoadRomTests
    {
        MainWindowViewModel ViewModel;
        MockDataService DataService;
        MockDialogService DialogService;

        public LoadRomTests()
        {
            DataService = new MockDataService();
            DataService.Save(PokemonId.Pikachu, new MockPokemon());
            DataService.Save(MoveId.Thunderbolt, new Move());
            DataService.Save(AbilityId.Static, new Ability());
            DataService.Save(WarriorSkillId.Ambition, new WarriorSkill());
            DialogService = new MockDialogService()
            {
                RequestRomFileReturnBool = true,
                RequestRomFileOutString = "Hello",
                ShowMessageBoxReturn = System.Windows.MessageBoxResult.OK
            };
            ViewModel = new MainWindowViewModel(DataService, DialogService);
        }

        [Fact]
        public void LoadRomShowsDialogsAndCallsLoad()
        {
            ViewModel.LoadRomCommand.Execute(null);

            Assert.Equal(1, DialogService.ShowMessageBoxCallCount);
            Assert.Equal(1, DialogService.RequestRomFileCallCount);
            Assert.Single(DataService.LoadRomCallLog);
            Assert.Equal(DialogService.RequestRomFileOutString, DataService.LoadRomCallLog.Dequeue());
        }
    }
}
