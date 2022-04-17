using FluentAssertions;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;

namespace RanseiLink.CoreTests.ModelTests;

public class BuildingTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Building a = new Building(new byte[]
        {
                0x53, 0x61, 0x63, 0x72,
                0x65, 0x64, 0x20, 0x52,
                0x75, 0x69, 0x6E, 0x73,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x77,
                0x77, 0x77, 0x77, 0x05,
                0x04, 0x03, 0x77, 0x00,
                0x14, 0x0A, 0x00, 0x00,
                0x14, 0x26, 0x93, 0x99
        });

        a.Name.Should().Be("Sacred Ruins");
        a.Kingdom.Should().Be(KingdomId.Aurora);
        a.BattleConfig1.Should().Be(BattleConfigId.SacredRuins);
        a.BattleConfig2.Should().Be(BattleConfigId.SacredRuins);
        a.BattleConfig3.Should().Be(BattleConfigId.SacredRuins);
        a.Sprite1.Should().Be(BuildingSpriteId.SacredRuins);
        a.Sprite2.Should().Be(BuildingSpriteId.SacredRuins);
        a.Sprite3.Should().Be(BuildingSpriteId.SacredRuins);
        a.Function.Should().Be(BuildingFunctionId.Battle);

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Building a = new Building
        {
            Name = "Sacred Ruins",
            Kingdom = KingdomId.Cragspur,
            BattleConfig1 = BattleConfigId.Cragspur,
            BattleConfig2 = BattleConfigId.SkyGarden_1,
            BattleConfig3 = BattleConfigId.Terrera,
            Sprite1 = BuildingSpriteId.SnowyMountain_1,
            Sprite2 = BuildingSpriteId.UndergroundMine_1,
            Sprite3 = BuildingSpriteId.Ravine_2,
            Function = BuildingFunctionId.ObtainGold
        };

        a.Name.Should().Be("Sacred Ruins");
        a.Kingdom.Should().Be(KingdomId.Cragspur);
        a.BattleConfig1.Should().Be(BattleConfigId.Cragspur);
        a.BattleConfig2.Should().Be(BattleConfigId.SkyGarden_1);
        a.BattleConfig3.Should().Be(BattleConfigId.Terrera);
        a.Sprite1.Should().Be(BuildingSpriteId.SnowyMountain_1);
        a.Sprite2.Should().Be(BuildingSpriteId.UndergroundMine_1);
        a.Sprite3.Should().Be(BuildingSpriteId.Ravine_2);
        a.Function.Should().Be(BuildingFunctionId.ObtainGold);

        // Add Array equal test when possible
    }
}
