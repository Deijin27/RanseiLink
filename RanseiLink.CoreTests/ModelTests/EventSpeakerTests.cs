using FluentAssertions;
using RanseiLink.Core.Models;
using Xunit;

namespace RanseiLink.CoreTests.ModelTests;

public class EventSpeakerTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        EventSpeaker a = new EventSpeaker(new byte[]
        {
                0x53, 0x68, 0x6F, 0x70, 0x6B, 0x65, 0x65, 0x70, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70
        });

        a.Name.Should().Be("Shopkeeper");
        a.Sprite.Should().Be(112);
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        EventSpeaker b = new EventSpeaker
        {
            Name = "Plant Owner",
            Sprite = 129
        };

        b.Name.Should().Be("Plant Owner");
        b.Sprite.Should().Be(129);
        byte[] expected = new byte[] { 0x50, 0x6C, 0x61, 0x6E, 0x74, 0x20, 0x4F, 0x77, 0x6E, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x81 };

        b.Data.Should().Equal(expected);
    }
}
