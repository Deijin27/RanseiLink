using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models
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

        public uint HpIv
        {
            get => GetUInt32(1, 5, 0);
            set => SetUInt32(1, 5, 0, value);
        }

        public uint AtkIv
        {
            get => GetUInt32(1, 5, 5);
            set => SetUInt32(1, 5, 5, value);
        }

        public uint DefIv
        {
            get => GetUInt32(1, 5, 10);
            set => SetUInt32(1, 5, 10, value);
        }

        public uint SpeIv
        {
            get => GetUInt32(1, 5, 15);
            set => SetUInt32(1, 5, 15, value);
        }

        public IScenarioPokemon Clone()
        {
            return new ScenarioPokemon((byte[])Data.Clone());
        }
    }
}
