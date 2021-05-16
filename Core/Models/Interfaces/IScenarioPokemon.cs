using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IScenarioPokemon : IDataWrapper
    {
        PokemonId Pokemon { get; }
    }
}