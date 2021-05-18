using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Xunit;

namespace Tests.ModelTests
{
    public class KingdomTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IKingdom a = new Kingdom(new byte[]
            {
                0x41, 0x75, 0x72, 0x6F, 
                0x72, 0x61, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x8C, 
                0x41, 0x10, 0x00, 0x00, 
                0x31, 0x10, 0x12, 0x05, 
                0x42, 0x44, 0x00, 0x2E,
            });

            Assert.Equal("Aurora", a.Name);
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            var aurora = new byte[]
            {
                0x41, 0x75, 0x72, 0x6F,
                0x72, 0x61, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x8C,
                0x41, 0x10, 0x00, 0x00,
                0x31, 0x10, 0x12, 0x05,
                0x42, 0x44, 0x00, 0x2E,
            };

            IKingdom a = new Kingdom
            {
                Name = "Aurora",
            };

            Assert.Equal("Aurora", a.Name);
        }
    }
}
