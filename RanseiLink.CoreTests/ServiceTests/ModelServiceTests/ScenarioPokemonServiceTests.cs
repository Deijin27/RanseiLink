using FluentAssertions;
using Moq;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System;
using Xunit;

namespace RanseiLink.CoreTests.ServiceTests.ModelServiceTests;

/// <summary>
/// WARNING: Model service tests require an unchanged mod at the location of the test mod folder.
/// </summary>
public class ScenarioPokemonServiceTests
{
    private readonly IScenarioPokemonService service;

    public ScenarioPokemonServiceTests()
    {
        service = new ScenarioPokemonService(new ModInfo() { FolderPath = TestConstants.TestModFolder }, new Mock<IPokemonService>().Object);
    }

    [Fact]
    public void ReadsCorrectValues()
    {
        var eevee = service.Retrieve((int)ScenarioId.TheLegendOfRansei).Retrieve(0);
        eevee.Data.Should().Equal(new byte[] { 0x00, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0x87, 0x04 });

        var oshawott = service.Retrieve((int)ScenarioId.TheLegendOfRansei).Retrieve(5);
        oshawott.Data.Should().Equal(new byte[] { 0x69, 0x00, 0xBE, 0x09, 0xEF, 0xBD, 0x57, 0x02 });

        var placeholder = service.Retrieve((int)ScenarioId.TheLegendOfRansei).Retrieve(199);
        placeholder.Data.Should().Equal(new byte[] { 0xC8, 0x00, 0xA4, 0x1A, 0xEF, 0xBD, 0x07, 0x08 });

        var fromAnotherScenario = service.Retrieve((int)ScenarioId.RanseisGreatestBeauty).Retrieve(1);
        fromAnotherScenario.Data.Should().Equal(new byte[] { 0x47, 0x00, 0xFC, 0x03, 0xEF, 0xBD, 0xC7, 0x02 });
    }

    [Fact]
    public void OutOfRangeThrowsException()
    {
        Action negativeTest = () => service.Retrieve((int)ScenarioId.TheLegendOfRansei).Retrieve(-1);
        negativeTest.Should().Throw<ArgumentOutOfRangeException>();

        Action largeTest = () => service.Retrieve((int)ScenarioId.TheLegendOfRansei).Retrieve(200);
        largeTest.Should().Throw<ArgumentOutOfRangeException>();

        Action largeScenarioTest = () => service.Retrieve(20).Retrieve(2);
        largeScenarioTest.Should().Throw<ArgumentOutOfRangeException>();
    }

}
