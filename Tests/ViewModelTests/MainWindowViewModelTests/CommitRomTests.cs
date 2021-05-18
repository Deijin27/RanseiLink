using Core.Enums;
using Core.Models;
using RanseiWpf.ViewModels;
using Tests.Mocks;
using Xunit;

namespace Tests.ViewModelTests.MainWindowViewModelTests
{
    public class CommitRomTests
    {
        MainWindowViewModel ViewModel;
        MockDataService DataService;
        MockDialogService DialogService;

        public CommitRomTests()
        {
            DataService = new MockDataService();
            DataService.Save(PokemonId.Pikachu, new MockPokemon());
            DataService.Save(MoveId.Thunderbolt, new Move());
            DataService.Save(AbilityId.Static, new Ability());
            DialogService = new MockDialogService()
            {
                RequestRomFileReturnBool = true,
                RequestRomFileOutString = "Hello",
                ShowMessageBoxReturn = System.Windows.MessageBoxResult.OK
            };
            ViewModel = new MainWindowViewModel(DataService, DialogService);
        }
        
        [Fact]
        public void CommitRomShowsDialogsAndCallsCommit()
        {
            Assert.True(ViewModel.PokemonVm.Cache.ContainsKey(PokemonId.Pikachu));
            ViewModel.PokemonVm.Cache[PokemonId.Pikachu].Ability1 = AbilityId.Parry;
            Assert.NotEqual(AbilityId.Parry, DataService.Retrieve(PokemonId.Pikachu).Ability1);

            ViewModel.CommitRomCommand.Execute(null);

            Assert.Equal(1, DialogService.ShowMessageBoxCallCount);
            Assert.Equal(1, DialogService.RequestRomFileCallCount);
            Assert.Single(DataService.CommitRomCallLog);
            Assert.Equal(DialogService.RequestRomFileOutString, DataService.CommitRomCallLog.Dequeue());
            Assert.Equal(AbilityId.Parry, DataService.Retrieve(PokemonId.Pikachu).Ability1);
        }
    }
}
