using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.CoreTests.ModelTests;

public class KingdomTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Kingdom a = new Kingdom(new byte[]
        {
                0x41, 0x75, 0x72, 0x6F,
                0x72, 0x61, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x8C,
                0x41, 0x10, 0x00, 0x00,
                0x31, 0x10, 0x12, 0x05,
                0x42, 0x44, 0x00, 0x2E,
        });

        a.Name.Should().Be("Aurora");
        a.BattleConfig.Should().Be(BattleConfigId.Aurora);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Kingdom a = new Kingdom
        {
            Name = "Aurora",
            BattleConfig = BattleConfigId.Yakasha
        };

        a.Name.Should().Be("Aurora");
        a.BattleConfig.Should().Be(BattleConfigId.Yakasha);
    }
}
