using Xunit;
using CliFx.Infrastructure;
using RanseiConsole.Commands;
using System.IO;
using Core.Services;
using Core.Enums;
using System;
using Tests.Mocks;
using RanseiConsole.Services;

namespace Tests.ScriptTests
{
    public class ScriptTests
    {
        readonly string TestScriptFolder = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ScriptTests).Assembly.CodeBase).AbsolutePath), "ScriptTests");

        [Fact]
        public async void ScriptInteractWithRomSuccessfully()
        {
            ModInfo modInfo = new ModInfo();

            MockCoreAppServices coreServices = new MockCoreAppServices();
            MockPokemonService mockPokemonService = new MockPokemonService();
            coreServices.DataServiceReturn[modInfo] = new MockDataService()
            {
                Pokemon = mockPokemonService
            };

            ConsoleAppServices.Instance = new MockConsoleAppServices()
            {
                CurrentMod = modInfo,
                CoreServices = coreServices
            };

            var input = new MockPokemon() { Type1 = TypeId.Grass };
            mockPokemonService.RetrieveReturn[PokemonId.Pikachu] = input;

            // Create 

            var console = new FakeInMemoryConsole();

            string file = Path.Combine(TestScriptFolder, "SetPropertyAndSaveTest.lua");

            var command = new LuaCommand()
            {
                FilePath = file,
            };

            // Execute

            await command.ExecuteAsync(console);

            // Test Changes Occurred

            Assert.Single(mockPokemonService.RetrieveCalls);
            Assert.Equal(PokemonId.Pikachu, mockPokemonService.RetrieveCalls.Dequeue());
            Assert.Single(mockPokemonService.SaveCalls);
            var (callId, callModel) = mockPokemonService.SaveCalls.Dequeue();
            Assert.Equal(PokemonId.Pikachu, callId);
            Assert.Same(input, callModel);
            Assert.Equal(TypeId.Electric, callModel.Type1);
        }
    }
}
