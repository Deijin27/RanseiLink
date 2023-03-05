using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.CoreTests.ModelTests;

public class EventSpeakerTests
{
    [Fact]
    public void AccessorsReturnCorrectValues_VPYT()
    {
        EventSpeaker a = new EventSpeaker(new byte[]
        {
            0x53, 0x68, 0x6F, 0x70, 
            0x6B, 0x65, 0x65, 0x70, 
            0x65, 0x72, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 
            0x00, 0x70
        },
        ConquestGameCode.VPYT);

        a.Name.Should().Be("Shopkeeper");
        a.Sprite.Should().Be(112);
    }

    [Fact]
    public void AccessorsReturnCorrectValues_VPYJ()
    {
        EventSpeaker a = new EventSpeaker(new byte[]
        {
            0x82, 0xC4, 0x82, 0xF1, 
            0x82, 0xA2, 0x82, 0xF1, 
            0x00, 0x00, 0x00, 0x70
        },
        ConquestGameCode.VPYJ);

        a.Name.Should().Be("てんいん");
        a.Sprite.Should().Be(112);
    }

    [Theory]
    [InlineData(ConquestGameCode.VPYT, "Plant Owner", new byte[] { 0x50, 0x6C, 0x61, 0x6E, 0x74, 0x20, 0x4F, 0x77, 0x6E, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x81 })]
    [InlineData(ConquestGameCode.VPYJ, "オーナー", new byte[] { 0x83, 0x49, 0x81, 0x5B, 0x83, 0x69, 0x81, 0x5B, 0x00, 0x00, 0x00, 0x81 })]
    public void AccessorsSetCorrectValues(ConquestGameCode gameCode, string name, byte[] expected)
    {
        EventSpeaker b = new EventSpeaker(gameCode)
        {
            Name = name,
            Sprite = 129
        };

        b.Name.Should().Be(name);
        b.Sprite.Should().Be(129);

        b.Data.Should().Equal(expected);
    }
}
