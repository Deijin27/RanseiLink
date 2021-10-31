using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System;
using Xunit;

namespace RanseiLink.Tests.ServiceTests.ModelServiceTests
{
    /// <summary>
    /// WARNING: Model service tests require an unchanged mod at the location of the test mod folder.
    /// </summary>
    public class ScenarioPokemonServiceTests
    {
        private readonly IScenarioPokemonService service;

        public ScenarioPokemonServiceTests()
        {
            service = new ScenarioPokemonService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        }

        [Fact]
        public void ReadsCorrectValues()
        {
            var eevee = service.Retrieve(ScenarioId.TheLegendOfRansei, 0);
            Assert.Equal(new byte[] { 0x00, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0x87, 0x04 }, eevee.Data);

            var oshawott = service.Retrieve(ScenarioId.TheLegendOfRansei, 5);
            Assert.Equal(new byte[] { 0x69, 0x00, 0xBE, 0x09, 0xEF, 0xBD, 0x57, 0x02 }, oshawott.Data);

            var placeholder = service.Retrieve(ScenarioId.TheLegendOfRansei, 199);
            Assert.Equal(new byte[] { 0xC8, 0x00, 0xA4, 0x1A, 0xEF, 0xBD, 0x07, 0x08 }, placeholder.Data);

            var fromAnotherScenario = service.Retrieve(ScenarioId.RanseisGreatestBeauty, 1);
            Assert.Equal(new byte[] { 0x47, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0xC7, 0x02 }, fromAnotherScenario.Data);
        }

        [Fact]
        public void OutOfRangeThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => service.Retrieve(ScenarioId.TheLegendOfRansei, -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => service.Retrieve(ScenarioId.TheLegendOfRansei, 200));

            Assert.Throws<ArgumentOutOfRangeException>(() => service.Retrieve((ScenarioId)20, 2));
        }

    }
}
