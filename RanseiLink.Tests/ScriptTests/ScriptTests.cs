using Xunit;
using CliFx.Infrastructure;
using RanseiLink.Console.Commands;
using System.IO;
using RanseiLink.Core.Services;
using RanseiLink.Core.Enums;
using System;
using RanseiLink.Tests.Mocks;
using RanseiLink.Console.Services;

namespace RanseiLink.Tests.ScriptTests
{
    public class ScriptTests
    {
        private readonly string TestScriptFolder = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ScriptTests).Assembly.Location).AbsolutePath), "ScriptTests");

        [Fact]
        public async void ScriptInteractWithRomSuccessfully()
        {
            MockPokemonService mockPokemonService = new MockPokemonService();

            MockCurrentModService mockCurrentModService = new MockCurrentModService()
            {
                TryGetDataServiceSucceed = true,
                TryGetDataServiceReturn = new MockDataService()
                {
                    Pokemon = mockPokemonService
                }
            };

            IServiceContainer container = new ServiceContainer();
            container.RegisterSingleton<ICurrentModService>(mockCurrentModService);


            var input = new MockPokemon() { Type1 = TypeId.Grass };
            mockPokemonService.RetrieveReturn[PokemonId.Pikachu] = input;

            // Create 

            var console = new FakeInMemoryConsole();

            string file = Path.Combine(TestScriptFolder, "SetPropertyAndSaveTest.lua");

            var command = new LuaCommand(container)
            {
                FilePath = file,
            };

            // Execute

            await command.ExecuteAsync(console);

            // Test Changes Occurred

            var retrieveItem = Assert.Single(mockPokemonService.RetrieveCalls);
            Assert.Equal(PokemonId.Pikachu, retrieveItem);
            var (callId, callModel) = Assert.Single(mockPokemonService.SaveCalls);
            Assert.Equal(PokemonId.Pikachu, callId);
            Assert.Same(input, callModel);
            Assert.Equal(TypeId.Electric, callModel.Type1);
        }
    }
}
