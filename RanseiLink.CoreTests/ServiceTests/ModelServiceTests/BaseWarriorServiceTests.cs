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
        var playerM1 = service.Retrieve((int)WarriorId.PlayerMale_1);
        Assert.Equal(
            new byte[]
            {
                    0x00, 0x00, 0xA2, 0xC9, 0xE0, 0x9B, 0xFF, 0x1F, 0x02, 0xA2, 0x00, 0xDD, 0x41, 0x5E, 0x90, 0x00, 0xFF, 0xFF, 0xFF, 0x07
            },
            playerM1.Data);

        var nobunaga1 = service.Retrieve((int)WarriorId.Nobunaga_1);
        Assert.Equal(
            new byte[]
            {
                    0x06, 0x02, 0xF4, 0xA0, 0x6E, 0x94, 0xBF, 0x05, 0x19, 0xD1, 0x03, 0x20, 0x59, 0x2F, 0xB7, 0x00, 0xC5, 0x02, 0xFC, 0x07
            },
            nobunaga1.Data);

        var mobB = service.Retrieve((int)WarriorId.Mob_B);
        Assert.Equal(
            new byte[]
            {
                    0x81, 0x81, 0xC2, 0x00, 0xE3, 0xA3, 0xFF, 0x1F, 0x00, 0x00, 0x7E, 0xDD, 0x32, 0x99, 0x2C, 0x00, 0xFF, 0xFF, 0xFF, 0x07
            },
            mobB.Data);
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
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.Retrieve(252));
    }

}
