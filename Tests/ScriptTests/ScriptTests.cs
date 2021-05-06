using Xunit;
using CliFx.Infrastructure;
using RanseiConsole.Commands;
using System.IO;
using Core.Services;
using Core.Enums;
using System;

namespace Tests.ScriptTests
{
    public class ScriptTests
    {
        const string TestScriptFolder = @"ScriptTests";


        [Fact]
        public async void ScriptInteractWithRomSuccessfully()
        {
            // Initialize
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Ransei/Tests/{nameof(ScriptInteractWithRomSuccessfully)}");
            var service = new DataService(folder);
            var pika = service.Retrieve(PokemonId.Pikachu);
            TypeId startType = pika.Type1;
            pika.Type1 = TypeId.Grass;
            service.Save(PokemonId.Pikachu, pika);

            // Create 

            var console = new FakeInMemoryConsole();

            var command = new LuaCommand()
            {
                FilePath = Path.Combine(TestScriptFolder, "SetPropertyAndSaveTest.lua"),
                Service = service
            };

            // Execute

            await command.ExecuteAsync(console);

            // Test Changes Occurred

            var elePika = service.Retrieve(PokemonId.Pikachu);
            Assert.Equal(TypeId.Electric, elePika.Type1);

            // Reset

            pika.Type1 = startType;
            service.Save(PokemonId.Pikachu, pika);
            

        }
    }
}
