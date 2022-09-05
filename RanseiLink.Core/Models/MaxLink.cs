using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models
{
    public class MaxLink : BaseDataWindow
    {
        public const int DataLength = 200;
        public MaxLink(byte[] data) : base(data, DataLength) { }
        public MaxLink() : this(new byte[DataLength]) { }

        public int GetMaxLink(PokemonId pokemon)
        {
            return GetMaxLink((int)pokemon);
        }

        public void SetMaxLink(PokemonId pokemon, int value)
        {
            SetMaxLink((int)pokemon, value);
        }

        public int GetMaxLink(int pokemon)
        {
            return GetByte(pokemon);
        }

        public void SetMaxLink(int pokemon, int value)
        {
            SetByte(pokemon, (byte)value);
        }
    }
}