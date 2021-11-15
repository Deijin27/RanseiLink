using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IScenarioAppearPokemon : IDataWrapper, ICloneable<IScenarioAppearPokemon>
{
    bool GetCanAppear(PokemonId id);
    void SetCanAppear(PokemonId id, bool canAppear);
}
