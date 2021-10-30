using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Xunit;

namespace Tests.ModelTests
{
    public class BuildingTests
    {
        [Fact]
        public void AccessorsReturnCorrectValues()
        {
            IBuilding b = new Building(new byte[]
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

            Assert.Equal("Sacred Ruins", b.Name);
            Assert.Equal(KingdomId.Aurora, b.Kingdom);
            
        }

        [Fact]
        public void AccessorsSetCorrectValues()
        {
            IBuilding b = new Building
            {
                Name = "Sacred Ruins",
                Kingdom = KingdomId.Cragspur
            };

            Assert.Equal("Sacred Ruins", b.Name);
            Assert.Equal(KingdomId.Cragspur, b.Kingdom);

            // Add Array equal test when possible
        }
    }
}
