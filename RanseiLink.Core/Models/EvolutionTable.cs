using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System;

namespace RanseiLink.Core.Models;

public class EvolutionTable : BaseDataWindow, IEvolutionTable
{
    public const int DataLength = 0x74; // DataLength == itemcount
    public EvolutionTable(byte[] data) : base(data, DataLength) { }
    public EvolutionTable() : this(new byte[DataLength]) { }

    public PokemonId GetEntry(int id)
    {
        return (PokemonId)GetByte(id);
    }

    public void SetEntry(int id, PokemonId pokemon)
    {
        SetByte(id, (byte)pokemon);
    }

    private void ValidateId(uint id)
    {
        if (id >= DataLength)
        {
            throw new ArgumentException($"{nameof(id)} of entry requested from {nameof(EvolutionTable)} must be less than {DataLength}");
        }
    }
}
