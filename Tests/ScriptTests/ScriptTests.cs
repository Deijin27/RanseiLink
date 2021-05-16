using Xunit;
using CliFx.Infrastructure;
using RanseiConsole.Commands;
using System.IO;
using Core.Services;
using Core.Enums;
using System;
using Tests.Mocks;

namespace Tests.ScriptTests
{
    public class ScriptTests
    {
        readonly string TestScriptFolder = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ScriptTests).Assembly.CodeBase).AbsolutePath), "ScriptTests");

        [Fact]
        public async void ScriptInteractWithRomSuccessfully()
        {

            // Initialize
            IDataService service = new MockDataService();
            service.Save(PokemonId.Pikachu, new MockPokemon() { Type1 = TypeId.Grass });

            // Create 

            var console = new FakeInMemoryConsole();

            string file = Path.Combine(TestScriptFolder, "SetPropertyAndSaveTest.lua");

            var command = new LuaCommand()
            {
                FilePath = file,
                Service = service
            };

            // Execute

            await command.ExecuteAsync(console);

            // Test Changes Occurred

            var elePika = service.Retrieve(PokemonId.Pikachu);
            Assert.Equal(TypeId.Electric, elePika.Type1);
        }

    }
}
