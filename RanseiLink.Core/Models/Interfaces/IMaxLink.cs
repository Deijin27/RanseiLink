using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces
{
    public interface IMaxLink : IDataWrapper
    {
        uint GetMaxLink(PokemonId pokemon);
        void SetMaxLink(PokemonId pokemon, uint value);
    }
}