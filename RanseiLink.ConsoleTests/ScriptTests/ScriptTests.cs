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

    [Fact]
    public void ScriptInteractWithPokemonService()
    {
        var mockPokemonService = new Mock<IPokemonService>();
        var mockModServiceContainer = new Mock<IModServiceContainer>();
        mockModServiceContainer.SetupGet(i => i.Pokemon).Returns(mockPokemonService.Object);

        var input = new Pokemon() { Type1 = TypeId.Grass };
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
    }
}
