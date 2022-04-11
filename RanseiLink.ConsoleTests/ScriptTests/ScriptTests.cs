using Xunit;
using System.IO;
using RanseiLink.Core.Services;
using RanseiLink.Core.Enums;
using System;
using Moq;
using RanseiLink.Console.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Models;

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

        string file = Path.Combine(TestScriptFolder, "ReadPokemonDataTest.lua");
        Assert.True(File.Exists(file), "Ensure that the test file exists");

        var luaService = new LuaService(mockModServiceContainer.Object);

        // assertions done within script
        luaService.RunScript(file);
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

        string file = Path.Combine(TestScriptFolder, "SetPropertyAndSaveTest.lua");
        Assert.True(File.Exists(file), "Ensure that the test file exists");

        var luaService = new LuaService(mockModServiceContainer.Object);

        luaService.RunScript(file);

        // Test Changes Occurred

        mockPokemonService.Verify(i => i.Retrieve(15), Times.Once());
        mockPokemonService.Verify(i => i.Save(), Times.Once());
        Assert.Equal(TypeId.Electric, input.Type1);
        Assert.Equal(34, input.Hp);
        Assert.True(input.IsLegendary);
        var evo = Assert.Single(input.Evolutions);
        Assert.Equal(PokemonId.Glaceon, evo);
    }

    [Fact]
    public void EnumerateWarriorsTest()
    {
        var warriorService = new BaseWarriorService(Path.Combine(TestModFolder, Constants.BaseBushouRomPath));

        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.BaseWarrior).Returns(warriorService);

        string file = Path.Combine(TestScriptFolder, "EnumerateWarriorsTest.lua");
        Assert.True(File.Exists(file), "Ensure that the test file exists");

        var luaService = new LuaService(mockModServiceContainer.Object);

        luaService.RunScript(file);

        foreach (var warrior in warriorService.Enumerate())
        {
            Assert.Equal(45, warrior.Wisdom);
            Assert.Equal(TypeId.Fire, warrior.Speciality1);
        }
    }

    [Fact]
    public void EnumerateByIdTest()
    {
        // this test is important because it was necessary to use Enumerable.Range()
        // rather than a generator to provide the IEnumerable
        // else luanet.each doesn't work

        var itemService = new ItemService(Path.Combine(TestModFolder, Constants.ItemRomPath));

        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.Item).Returns(itemService);

        string file = Path.Combine(TestScriptFolder, "EnumerateByIdTest.lua");
        Assert.True(File.Exists(file), "Ensure that the test file exists");

        var luaService = new LuaService(mockModServiceContainer.Object);

        luaService.RunScript(file);

        int id = 0;
        foreach (var item in itemService.Enumerate())
        {
            Assert.Equal(id.ToString(), item.Name);
            Assert.Equal(id, item.ShopPriceMultiplier);
            id++;
        }
    }
}
