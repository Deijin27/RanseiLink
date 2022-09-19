using Xunit;
using System.IO;
using RanseiLink.Core.Services;
using RanseiLink.Core.Enums;
using System;
using Moq;
using RanseiLink.Console.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Models;
using FluentAssertions;

namespace RanseiLink.ConsoleTests.ScriptTests;

public class ScriptTests
{
    private readonly string TestScriptFolder = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ScriptTests).Assembly.Location).AbsolutePath), "ScriptTests");

    public static readonly string TestModFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RanseiLink", "Test", "TestMod");

    [Fact]
    public void ReadPokemonDataTest()
    {
        var mockPokemonService = new Mock<IPokemonService>();
        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.Pokemon).Returns(mockPokemonService.Object);

        var input = new Pokemon()
        {
            Name = "Pikachu",
            Type1 = TypeId.Electric,
            Hp = 20,
            IsLegendary = false,
        };
        mockPokemonService.Setup(i => i.Retrieve(15)).Returns(input);

        // Create 

        string readPokemonDataTestFile = Path.Combine(TestScriptFolder, "ReadPokemonDataTest.lua");
        File.Exists(readPokemonDataTestFile).Should().BeTrue();

        var luaService = new LuaService(mockModServiceContainer.Object);

        // assertions done within script
        luaService.RunScript(readPokemonDataTestFile);
    }

    [Fact]
    public void ScriptInteractWithPokemonService()
    {
        var mockPokemonService = new Mock<IPokemonService>();
        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.Pokemon).Returns(mockPokemonService.Object);

        var input = new Pokemon() 
        { 
            Type1 = TypeId.Grass,
            Hp = 20,
            IsLegendary = false,
        };
        mockPokemonService.Setup(i => i.Retrieve(15)).Returns(input);

        // Create 

        string setPropertySaveTestFile = Path.Combine(TestScriptFolder, "SetPropertyAndSaveTest.lua");
        File.Exists(setPropertySaveTestFile).Should().BeTrue();

        var luaService = new LuaService(mockModServiceContainer.Object);

        luaService.RunScript(setPropertySaveTestFile);

        // Test Changes Occurred

        mockPokemonService.Verify(i => i.Retrieve(15), Times.Once());
        mockPokemonService.Verify(i => i.Save(), Times.Once());

        input.Type1.Should().Be(TypeId.Electric);
        input.Hp.Should().Be(34);
        input.IsLegendary.Should().BeTrue();
        input.Evolutions.Should().ContainSingle().Which.Should().Be(PokemonId.Glaceon);
    }

    [Fact]
    public void EnumerateWarriorsTest()
    {
        var warriorService = BaseWarriorService.Load(Path.Combine(TestModFolder, Constants.BaseBushouRomPath));

        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.BaseWarrior).Returns(warriorService);

        string enumerateWarriorsTestFile = Path.Combine(TestScriptFolder, "EnumerateWarriorsTest.lua");
        File.Exists(enumerateWarriorsTestFile).Should().BeTrue();

        var luaService = new LuaService(mockModServiceContainer.Object);

        luaService.RunScript(enumerateWarriorsTestFile);

        foreach (var warrior in warriorService.Enumerate())
        {
            warrior.Wisdom.Should().Be(45);
            warrior.Speciality1.Should().Be(TypeId.Fire);
        }
    }

    [Fact]
    public void EnumerateByIdTest()
    {
        // this test is important because it was necessary to use Enumerable.Range()
        // rather than a generator to provide the IEnumerable
        // else luanet.each doesn't work

        var itemService = ItemService.Load(Path.Combine(TestModFolder, Constants.ItemRomPath));

        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.Item).Returns(itemService);

        string enumerateByIdTestFile = Path.Combine(TestScriptFolder, "EnumerateByIdTest.lua");
        File.Exists(enumerateByIdTestFile).Should().BeTrue();

        var luaService = new LuaService(mockModServiceContainer.Object);

        luaService.RunScript(enumerateByIdTestFile);

        int id = 0;
        foreach (var item in itemService.Enumerate())
        {
            item.Name.Should().Be(id.ToString());
            item.ShopPriceMultiplier.Should().Be(id);
            id++;
        }
    }
}
