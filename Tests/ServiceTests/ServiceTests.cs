using Core.Services;
using Core.Enums;
using Xunit;
using System;
using System.IO;
using Tests.Mocks;
using Core.Models;

namespace Tests.ServiceTests
{
    public class ServiceTests
    {
        [Fact]
        void ServiceRetrieveAndSavePokemonTest()
        {
            IDataService service = new DataService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Ransei/Tests/{nameof(ServiceRetrieveAndSavePokemonTest)}"));

            var raichu = service.Retrieve(PokemonId.Raichu);

            var mockRaichu = new MockPokemon()
            {
                Data = new byte[Pokemon.DataLength]
                {
                    0x43, 0x41, 0x6C, 0x5C, 0x21, 0x63, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
                    0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0x4C, 0x50, 0x69, 0xFE, 0x03, 0x18,
                    0x78, 0xC5, 0xEB, 0x76, 0x03, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
                }
            };

            service.Save(PokemonId.Raichu, mockRaichu);

            var savedAndRetrievedRaichu = service.Retrieve(PokemonId.Raichu);

            Assert.Equal(mockRaichu.Data, savedAndRetrievedRaichu.Data);

            service.Save(PokemonId.Raichu, raichu);

        }
    }
}
