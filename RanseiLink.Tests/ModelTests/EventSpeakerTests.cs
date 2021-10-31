using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Models.Interfaces;
using Xunit;

namespace RanseiLink.Tests.ModelTests
{
    public class EventSpeakerTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IEventSpeaker b = new EventSpeaker(new byte[]
            {
                0x53, 0x68, 0x6F, 0x70, 0x6B, 0x65, 0x65, 0x70, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70
            });

            Assert.Equal("Shopkeeper", b.Name);
            Assert.Equal(WarriorSpriteId.Tsunehisa, b.Sprite);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            IEventSpeaker b = new EventSpeaker
            {
                Name = "Plant Owner",
                Sprite = WarriorSpriteId.Tomonori
            };

            Assert.Equal("Plant Owner", b.Name);
            Assert.Equal(WarriorSpriteId.Tomonori, b.Sprite);

            byte[] expected = new byte[] { 0x50, 0x6C, 0x61, 0x6E, 0x74, 0x20, 0x4F, 0x77, 0x6E, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x81 };
            Assert.Equal(expected, b.Data);
        }
    }
}
