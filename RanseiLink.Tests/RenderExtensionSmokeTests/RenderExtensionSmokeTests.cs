using CliFx.Infrastructure;
using Xunit;
using RanseiLink.Console;
using RanseiLink.Core.Models;

namespace RanseiLink.Tests.RenderExtensionSmokeTests
{
    public class RenderExtensionSmokeTests
    {
        private readonly IConsole console;
        public RenderExtensionSmokeTests()
        {
            console = new FakeInMemoryConsole();
        }

        [Fact]
        public void PokemonRunsWithoutException()
        {
            console.Render(new Pokemon(), 0);
        }
        [Fact]
        public void MoveRunsWithoutException()
        {
            console.Render(new Move(), 0);
        }
        [Fact]
        public void AbilityRunsWithoutException()
        {
            console.Render(new Ability(), 0);
        }
        [Fact]
        public void WarriorSkillRunsWithoutException()
        {
            console.Render(new WarriorSkill(), 0);
        }
        [Fact]
        public void GimmickRunsWithoutException()
        {
            console.Render(new Gimmick(), 0);
        }
        [Fact]
        public void BuildingRunsWithoutException()
        {
            console.Render(new Building(), 0);
        }
        [Fact]
        public void ItemRunsWithoutException()
        {
            console.Render(new Item(), 0);
        }
        [Fact]
        public void KingdomRunsWithoutException()
        {
            console.Render(new Kingdom(), 0);
        }
        [Fact]
        public void WarriorMaxLinkRunsWithoutException()
        {
            console.Render(new MaxLink(), 0);
        }
        [Fact]
        public void ScenarioPokemonRunsWithoutException()
        {
            console.Render(new ScenarioPokemon(), 0, 0);
        }
        [Fact]
        public void ScenarioWarriorRunsWithoutException()
        {
            console.Render(new ScenarioWarrior(), 0, 0);
        }
        [Fact]
        public void EvolutionTableRunsWithoutException()
        {
            console.Render(new EvolutionTable());
        }
        [Fact]
        public void WarriorNameTableRunsWithoutException()
        {
            console.Render(new WarriorNameTable());
        }
        [Fact]
        public void BaseWarriorRunsWithoutException()
        {
            console.Render(new BaseWarrior(), 0);
        }
        [Fact]
        public void ScenarioAppearPokemonRunsWithoutException()
        {
            console.Render(new ScenarioAppearPokemon(), 0);
        }
        [Fact]
        public void EventSpeakerRunsWithoutException()
        {
            console.Render(new EventSpeaker(), 0);
        }
        [Fact]
        public void ScenarioKingdomRunsWithoutException()
        {
            console.Render(new ScenarioKingdom(), 0);
        }
    }
}
