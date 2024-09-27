namespace RanseiLink.Core.Models;

public partial class ScenarioWarrior : BaseDataWindow
{
    public int GetScenarioPokemon(int id)
    {
        if (id > 7 || id < 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(id)} is out of range. Scenario warriors only have 8 pokemon");
        }
        return GetUInt16(0xE + id * 2);
    }

    public void SetScenarioPokemon(int id, int value)
    {
        if (id > 7 || id < 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(id)} is out of range. Scenario warriors only have 8 pokemon");
        }
        SetUInt16(0xE + id * 2, (ushort)value);
    }


    public void MakeScenarioPokemonDefault(int id)
    {
        SetScenarioPokemon(id, DefaultScenarioPokemon);
    }

    public bool ScenarioPokemonIsDefault(int id)
    {
        return GetScenarioPokemon(id) == DefaultScenarioPokemon;
    }

    public const ushort DefaultScenarioPokemon = 1100;
}