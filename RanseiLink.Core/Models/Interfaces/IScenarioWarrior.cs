using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces
{
    public interface IScenarioWarrior : IDataWrapper, ICloneable<IScenarioWarrior>
    {
        WarriorId Warrior { get; set; }
        uint ScenarioPokemon { get; set; }
        bool ScenarioPokemonIsDefault { get; set; }
    }
}