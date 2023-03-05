using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System;
using System.IO;

namespace RanseiLink.CoreTests.ServiceTests.ModelServiceTests;

/// <summary>
/// WARNING: Model service tests require an unchanged mod at the location of the test mod folder.
/// </summary>
public class PokemonServiceTests
{

    public PokemonServiceTests()
    {
    }

    [Fact]
    public void ReadsCorrectValues()
    {
        IPokemonService service = new PokemonService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var eevee = service.Retrieve((int)PokemonId.Eevee);

        var expectedEeveeData = new byte[]
        {
            0x45, 0x65, 0x76, 0x65, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xDC, 0x64, 0x02, 0x45, 0x73, 0xDC, 0x31, 0x07, 0xE0, 0x17, 0xF8, 0x0F,
            0x5B, 0x34, 0x20, 0x51, 0xFF, 0xFF, 0xFF, 0x27, 0x00, 0x30, 0x40, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00
        };
        eevee.Data.Should().Equal(expectedEeveeData);
       

        var leafeon = service.Retrieve((int)PokemonId.Leafeon);

        var expectedLeafeonData = new byte[]
        {
            0x4C, 0x65, 0x61, 0x66, 0x65, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF0, 0x48, 0x01, 0x49, 0xE1, 0x1C, 0x33, 0x8C, 0xE4, 0xAF, 0xF8, 0x0F,
            0x61, 0x38, 0xD4, 0x50, 0x5E, 0xFE, 0x13, 0x20, 0x78, 0xC5, 0xAB, 0x75, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00
        };
        leafeon.Data.Should().Equal(expectedLeafeonData);

        var rayquaza = service.Retrieve((int)PokemonId.Rayquaza);

        var expectedRayquazaData = new byte[]
        {
            0x52, 0x61, 0x79, 0x71, 0x75, 0x61, 0x7A, 0x61, 0x00, 0x00, 0x00, 0x03, 0x45, 0x65, 0x02, 0x52, 0x36, 0xF9, 0x82, 0xCC, 0x2E, 0xA1, 0xF9, 0x0F,
            0x42, 0x00, 0x01, 0x52, 0xFF, 0xFF, 0xFF, 0x27, 0x78, 0xC5, 0x2B, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8C, 0x00, 0x00, 0x00
        };
        rayquaza.Data.Should().Equal(expectedRayquazaData);
    }

    [Fact]
    public void LoadsEvolutionsCorrectly()
    {
        IPokemonService service = new PokemonService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var eevee = service.Retrieve((int)PokemonId.Eevee);

        eevee.Evolutions.Should().HaveCount(7);
        eevee.Evolutions[0].Should().Be(PokemonId.Vaporeon);
        eevee.Evolutions[1].Should().Be(PokemonId.Jolteon);

        var leafeon = service.Retrieve((int)PokemonId.Leafeon);
        leafeon.Evolutions.Should().BeEmpty();

        var dewott = service.Retrieve((int)PokemonId.Dewott);
        dewott.Evolutions.Should().ContainSingle().Which.Should().Be(PokemonId.Samurott);
    }

    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        string temp = Path.GetTempFileName();
        string source = Path.Combine(TestConstants.TestModFolder, Constants.PokemonRomPath);
        File.Copy(source, temp, true);
        IPokemonService service = PokemonService.Load(temp);
        service.Save();
        byte[] expected = File.ReadAllBytes(source);
        byte[] actual = File.ReadAllBytes(temp);
        actual.Should().Equal(expected);
        File.Delete(temp);
    }

    [Fact]
    public void OutOfRangeThrowsException()
    {
        IPokemonService service = new PokemonService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        Action retrieveTooBigId = () => service.Retrieve(200);
        retrieveTooBigId.Should().Throw<ArgumentOutOfRangeException>();
    }

}
