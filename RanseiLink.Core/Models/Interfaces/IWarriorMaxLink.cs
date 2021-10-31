using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces
{
    public interface IWarriorMaxLink : IDataWrapper
    {
        uint GetMaxLink(PokemonId pokemon);
        void SetMaxLink(PokemonId pokemon, uint value);
    }
}