using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class EvolutionTable : BaseDataWindow, IEvolutionTable
    {
        public const int DataLength = 0x74; // DataLength == itemcount
        public EvolutionTable(byte[] data) : base(data, DataLength) { }
        public EvolutionTable() : this(new byte[DataLength]) { }

        public PokemonId GetEntry(int index)
        {
            return (PokemonId)GetByte(index);
        }

        public void SetEntry(int index, PokemonId pokemon)
        {
            SetByte(index, (byte)pokemon);
        }
    }
}
