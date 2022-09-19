using FluentAssertions;
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
        var expectedPlayerM1Data = new byte[] { 0x00, 0x00, 0xA2, 0xC9, 0xE0, 0x9B, 0xFF, 0x1F, 0x02, 0xA2, 0x00, 0xDD, 0x41, 0x5E, 0x90, 0x00, 0xFF, 0xFF, 0xFF, 0x07 };
        playerM1.Data.Should().Equal(expectedPlayerM1Data);

        var nobunaga1 = service.Retrieve((int)WarriorId.Nobunaga_1);
        var expectedNobunaga1Data = new byte[] { 0x06, 0x02, 0xF4, 0xA0, 0x6E, 0x94, 0xBF, 0x05, 0x19, 0xD1, 0x03, 0x20, 0x59, 0x2F, 0xB7, 0x00, 0xC5, 0x02, 0xFC, 0x07 };
        nobunaga1.Data.Should().Equal(expectedNobunaga1Data);
        
        var mobB = service.Retrieve((int)WarriorId.Mob_B);
        var expectedMobBData = new byte[] { 0x81, 0x81, 0xC2, 0x00, 0xE3, 0xA3, 0xFF, 0x1F, 0x00, 0x00, 0x7E, 0xDD, 0x32, 0x99, 0x2C, 0x00, 0xFF, 0xFF, 0xFF, 0x07 };
        mobB.Data.Should().Equal(expectedMobBData);
    }

    [Fact]
    public void LoadsNamesCorrectly()
    {
        IBaseWarriorService service = new BaseWarriorService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var nameTable = service.NameTable;

        var oichi = service.Retrieve((int)WarriorId.Oichi_1);
        nameTable.GetEntry(oichi.WarriorName).Should().Be("Oichi");

        var shingen = service.Retrieve((int)WarriorId.Shingen_2);
        nameTable.GetEntry(shingen.WarriorName).Should().Be("Shingen");
    }

    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        string temp = Path.GetTempFileName();
        string source = Path.Combine(TestConstants.TestModFolder, Constants.BaseBushouRomPath);
        File.Copy(source, temp, true);
        IBaseWarriorService service = BaseWarriorService.Load(temp);
        service.Save();
        byte[] expected = File.ReadAllBytes(source);
        byte[] actual = File.ReadAllBytes(temp);

        expected.Should().Equal(actual);

        File.Delete(temp);
    }

    [Fact]
    public void OutOfRangeThrowsException()
    {
        IBaseWarriorService service = new BaseWarriorService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        Action action = () => service.Retrieve(252);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

}
