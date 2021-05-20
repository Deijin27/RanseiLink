using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class ScenarioPokemon : BaseDataWindow, IScenarioPokemon
    {
        public const int DataLength = 8;
        public ScenarioPokemon(byte[] data) : base(data, DataLength) { }
        public ScenarioPokemon() : base(new byte[DataLength], DataLength) { }

        public PokemonId Pokemon
        {
            get => (PokemonId)GetByte(0);
            set => SetByte(0, (byte)value);
        }

        public AbilityId Ability
        {
            get => (AbilityId)GetUInt32(1, 8, 20);
            set => SetUInt32(1, 8, 20, (uint)value);
        }

        public IScenarioPokemon Clone()
        {
            return new ScenarioPokemon((byte[])Data.Clone());
        }
    }
}
