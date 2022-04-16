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
            return GetByte((int)pokemon);
        }

        public void SetMaxLink(PokemonId pokemon, int value)
        {
            SetByte((int)pokemon, (byte)value);
        }
    }
}