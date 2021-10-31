using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models
{
    public class WarriorMaxLink : BaseDataWindow, IWarriorMaxLink
    {
        public const int DataLength = 200;
        public WarriorMaxLink(byte[] data) : base(data, DataLength) { }
        public WarriorMaxLink() : this(new byte[DataLength]) { }

        public uint GetMaxLink(PokemonId pokemon)
        {
            return GetByte((int)pokemon);
        }

        public void SetMaxLink(PokemonId pokemon, uint value)
        {
            SetByte((int)pokemon, (byte)value);
        }
    }
}
