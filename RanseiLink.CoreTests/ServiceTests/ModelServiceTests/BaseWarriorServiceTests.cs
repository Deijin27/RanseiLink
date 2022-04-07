using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System;
using System.IO;
using Xunit;

namespace RanseiLink.CoreTests.ServiceTests.ModelServiceTests;

/// <summary>
/// WARNING: Model service tests require an unchanged mod at the location of the test mod folder.
/// </summary>
public class BaseWarriorServiceTests
{

    public BaseWarriorServiceTests()
    {
    }

    [Fact]
    public void ReadsCorrectValues()
    {
        IBaseWarriorService service = new BaseWarriorService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var eevee = service.Retrieve((int)PokemonId.Eevee);
        Assert.Equal(
            new byte[]
            {
                    0x45, 0x65, 0x76, 0x65, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xDC, 0x64, 0x02, 0x45, 0x73, 0xDC, 0x31, 0x07, 0xE0, 0x17, 0xF8, 0x0F,
                    0x5B, 0x34, 0x20, 0x51, 0xFF, 0xFF, 0xFF, 0x27, 0x00, 0x30, 0x40, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00
            },
            eevee.Data);

        var leafeon = service.Retrieve((int)PokemonId.Leafeon);
        Assert.Equal(
            new byte[]
            {
                    0x4C, 0x65, 0x61, 0x66, 0x65, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x2D, 0xF0, 0x48, 0x01, 0x49, 0xE1, 0x1C, 0x33, 0x8C, 0xE4, 0xAF, 0xF8, 0x0F,
                    0x61, 0x38, 0xD4, 0x50, 0x5E, 0xFE, 0x13, 0x20, 0x78, 0xC5, 0xAB, 0x75, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00
            },
            leafeon.Data);

        var rayquaza = service.Retrieve((int)PokemonId.Rayquaza);
        Assert.Equal(
            new byte[]
            {
                    0x52, 0x61, 0x79, 0x71, 0x75, 0x61, 0x7A, 0x61, 0x00, 0x00, 0x00, 0x03, 0x45, 0x65, 0x02, 0x52, 0x36, 0xF9, 0x82, 0xCC, 0x2E, 0xA1, 0xF9, 0x0F,
                    0x42, 0x00, 0x01, 0x52, 0xFF, 0xFF, 0xFF, 0x27, 0x78, 0xC5, 0x2B, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8C, 0x00, 0x00, 0x00
            },
            rayquaza.Data);
    }

    [Fact]
    public void LoadsNamesCorrectly()
    {
        IBaseWarriorService service = new BaseWarriorService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var nameTable = service.NameTable;

        var oichi = service.Retrieve((int)WarriorId.Oichi_1);
        Assert.Equal("Oichi", nameTable.GetEntry(oichi.WarriorName));

        var shingen = service.Retrieve((int)WarriorId.Shingen_2);
        Assert.Equal("Shingen", nameTable.GetEntry(shingen.WarriorName));
    }

    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        string temp = Path.GetTempFileName();
        string source = Path.Combine(TestConstants.TestModFolder, Constants.BaseBushouRomPath);
        File.Copy(source, temp, true);
        IBaseWarriorService service = new BaseWarriorService(temp);
        service.Save();
        byte[] expected = File.ReadAllBytes(source);
        byte[] actual = File.ReadAllBytes(temp);
        Assert.Equal(expected, actual);
        File.Delete(temp);
    }

    [Fact]
    public void OutOfRangeThrowsException()
    {
        IBaseWarriorService service = new BaseWarriorService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.Retrieve(200));
    }

}
