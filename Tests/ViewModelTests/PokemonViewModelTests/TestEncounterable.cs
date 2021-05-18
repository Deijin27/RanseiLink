using RanseiWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Mocks;
using Xunit;
using Core.Enums;

namespace Tests.ViewModelTests.PokemonViewModelTests
{
    public class TestEncounterable
    {
        PokemonViewModel ViewModel;

        public TestEncounterable()
        {
            var mock = new MockPokemon();
            mock.SetEncounterable(KingdomId.Avia, true, true);
            mock.SetEncounterable(KingdomId.Dragnor, false, true);
            mock.SetEncounterable(KingdomId.Fontaine, true, false);
            mock.SetEncounterable(KingdomId.Ignis, false, false);

            ViewModel = new PokemonViewModel()
            {
                Model = mock
            };
        }

        [Fact]
        public void ChangeLocation()
        {
            ViewModel.SelectedEncounterKingdom = KingdomId.Avia;
            Assert.True(ViewModel.EncounterableWithLevel2Area, "Assertion1");

            ViewModel.SelectedEncounterKingdom = KingdomId.Dragnor;
            Assert.True(ViewModel.EncounterableAtDefaultArea, "Assertion2");

            ViewModel.SelectedEncounterKingdom = KingdomId.Fontaine;
            Assert.False(ViewModel.EncounterableWithLevel2Area, "Assertion3");

            ViewModel.SelectedEncounterKingdom = KingdomId.Ignis;
            Assert.False(ViewModel.EncounterableAtDefaultArea, "Assertion4");
        }

        [Fact]
        public void ChangeEncounterable()
        {
            ViewModel.SelectedEncounterKingdom = KingdomId.Avia;

            ViewModel.EncounterableWithLevel2Area = false;
            Assert.False(ViewModel.EncounterableWithLevel2Area, "Assertion1");

            ViewModel.EncounterableWithLevel2Area = true;
            Assert.True(ViewModel.EncounterableWithLevel2Area, "Assertion2");

            ViewModel.SelectedEncounterKingdom = KingdomId.Fontaine;

            ViewModel.EncounterableAtDefaultArea = true;
            Assert.True(ViewModel.EncounterableAtDefaultArea, "Assertion3");

            ViewModel.EncounterableAtDefaultArea = false;
            Assert.False(ViewModel.EncounterableAtDefaultArea, "Assertion4");

        }
    }
}
