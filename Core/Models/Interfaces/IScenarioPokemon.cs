using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IScenarioPokemon : IDataWrapper, ICloneable<IScenarioPokemon>
    {
        PokemonId Pokemon { get; }
    }
}