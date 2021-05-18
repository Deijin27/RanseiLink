using Core.Enums;
using Core.Models;
using RanseiWpf.ViewModels;
using Tests.Mocks;
using Xunit;

namespace Tests.ViewModelTests.MainWindowViewModelTests
{
    public class SaveChangesTest
    {
        MainWindowViewModel ViewModel;
        MockDataService DataService;
        MockDialogService DialogService;

        public SaveChangesTest()
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
        public void SaveChangesSuceessful()
        {
            Assert.True(ViewModel.PokemonVm.Cache.ContainsKey(PokemonId.Pikachu));
            ViewModel.PokemonVm.Cache[PokemonId.Pikachu].Ability1 = AbilityId.Parry;
            Assert.NotEqual(AbilityId.Parry, DataService.Retrieve(PokemonId.Pikachu).Ability1);

            ViewModel.SaveChangesCommand.Execute(null);

            Assert.Equal(AbilityId.Parry, DataService.Retrieve(PokemonId.Pikachu).Ability1);
        }
    }
}
