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
            mock.SetEncounterable(LocationId.Avia, true, true);
            mock.SetEncounterable(LocationId.Dragnor, false, true);
            mock.SetEncounterable(LocationId.Fontaine, true, false);
            mock.SetEncounterable(LocationId.Ignis, false, false);

            ViewModel = new PokemonViewModel()
            {
                Model = mock
            };
        }

        [Fact]
        public void ChangeLocation()
        {
            ViewModel.SelectedEncounterLocation = LocationId.Avia;
            Assert.True(ViewModel.EncounterableWithLevel2Area, "Assertion1");

            ViewModel.SelectedEncounterLocation = LocationId.Dragnor;
            Assert.True(ViewModel.EncounterableAtDefaultArea, "Assertion2");

            ViewModel.SelectedEncounterLocation = LocationId.Fontaine;
            Assert.False(ViewModel.EncounterableWithLevel2Area, "Assertion3");

            ViewModel.SelectedEncounterLocation = LocationId.Ignis;
            Assert.False(ViewModel.EncounterableAtDefaultArea, "Assertion4");
        }

        [Fact]
        public void ChangeEncounterable()
        {
            ViewModel.SelectedEncounterLocation = LocationId.Avia;

            ViewModel.EncounterableWithLevel2Area = false;
            Assert.False(ViewModel.EncounterableWithLevel2Area, "Assertion1");

            ViewModel.EncounterableWithLevel2Area = true;
            Assert.True(ViewModel.EncounterableWithLevel2Area, "Assertion2");

            ViewModel.SelectedEncounterLocation = LocationId.Fontaine;

            ViewModel.EncounterableAtDefaultArea = true;
            Assert.True(ViewModel.EncounterableAtDefaultArea, "Assertion3");

            ViewModel.EncounterableAtDefaultArea = false;
            Assert.False(ViewModel.EncounterableAtDefaultArea, "Assertion4");

        }
    }
}
