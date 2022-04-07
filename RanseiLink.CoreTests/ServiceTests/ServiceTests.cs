using RanseiLink.Core.Services;
using RanseiLink.Core.Enums;
using Xunit;
using System;
using System.IO;
using RanseiLink.Core.Models;

namespace RanseiLink.CoreTests.ServiceTests;

public class ServiceTests
{
    //IDataService Service;

    //public ServiceTests()
    //{
    //    Service = new DataService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Ransei/Tests/{nameof(ServiceRetrieveAndSavePokemonTest)}"));
    //}

    //[Fact]
    //void ServiceRetrieveAndSavePokemonTest()
    //{
    //    // Initialize

    //    IPokemon mockRaichu1 = new MockPokemon()
    //    {
    //        Data = new byte[Pokemon.DataLength]
    //        {
    //            0x43, 0x41, 0x6C, 0x5C, 0x21, 0x63, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF6, 0x9C, 0x01, 0x49,
    //            0xFF, 0xE4, 0x52, 0x0A, 0xCA, 0x4C, 0xF9, 0x0F, 0x1B, 0xC4, 0x4C, 0x50, 0x69, 0xFE, 0x03, 0x18,
    //            0x78, 0xC5, 0xEB, 0x76, 0x03, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00
    //        }
    //    };

    //    IPokemon mockRaichu2 = new MockPokemon()
    //    {
    //        Data = new byte[Pokemon.DataLength]
    //        {
    //            0x53, 0x44, 0x5C, 0x56, 0x67, 0x63, 0x65, 0x10, 0x02, 0x35, 0x00, 0x24, 0xF6, 0x4C, 0x01, 0x59,
    //            0xF1, 0xE2, 0x54, 0x55, 0x35, 0x4C, 0xF9, 0x0F, 0x1B, 0xC8, 0x4C, 0x23, 0x67, 0xFF, 0x03, 0x38,
    //            0x78, 0xC5, 0xE6, 0x76, 0x33, 0x60, 0x45, 0x30, 0x94, 0x00, 0x00, 0x67, 0x47, 0x71, 0x37, 0x8F
    //        }
    //    };

    //    Assert.NotEqual(mockRaichu1, mockRaichu2);

    //    // One time

    //    Service.Save(PokemonId.Raichu, mockRaichu1);

    //    var savedAndRetrievedRaichu1 = Service.Retrieve(PokemonId.Raichu);

    //    Assert.Equal(mockRaichu1.Data, savedAndRetrievedRaichu1.Data);

    //    // Second time

    //    Service.Save(PokemonId.Raichu, mockRaichu2);

    //    var savedAndRetrievedRaichu2 = Service.Retrieve(PokemonId.Raichu);

    //    Assert.Equal(mockRaichu2.Data, savedAndRetrievedRaichu2.Data);

    //}
}
