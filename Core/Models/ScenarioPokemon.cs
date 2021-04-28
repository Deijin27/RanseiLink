using Core.Enums;
using Core.Structs;

namespace Core.Models
{
    public class ScenarioPokemon : BaseDataWindow
    {
        public const int DataLength = 8;
        public ScenarioPokemon(byte[] data) : base(data, DataLength) { }
        public ScenarioPokemon() : base(new byte[DataLength], DataLength) { }

        public PokemonId Pokemon
        {
            get => (PokemonId)GetByte(0);
        }
    }
}
