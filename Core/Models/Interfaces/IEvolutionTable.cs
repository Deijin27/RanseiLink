using Core.Enums;

namespace Core.Models.Interfaces
{
    public interface IEvolutionTable : IDataWrapper
    {
        PokemonId GetEntry(int index);
        void SetEntry(int index, PokemonId pokemon);
    }
}