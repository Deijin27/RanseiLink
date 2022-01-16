using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IScenarioWarrior : IDataWrapper, ICloneable<IScenarioWarrior>
{
    WarriorId Warrior { get; set; }
    WarriorClassId Class { get; set; }
    KingdomId Kingdom { get; set; }
    uint Army { get; set; }

    public ushort ScenarioPokemon
    {
        get => GetScenarioPokemon(0);
        set => SetScenarioPokemon(0, value);
    }

    void MakeScenarioPokemonDefault(int id);
    bool ScenarioPokemonIsDefault(int id);
    ushort GetScenarioPokemon(int id);
    void SetScenarioPokemon(int id, ushort value);
}
