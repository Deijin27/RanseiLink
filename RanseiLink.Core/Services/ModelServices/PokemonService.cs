using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.Globalization;

namespace RanseiLink.Core.Services.ModelServices;

public partial class PokemonService
{
    private const int __defaultEvoId = 1400;
    private const long __evoTableOffset = 0x25C0;

    public override void PostLoad(Stream stream)
    {
        // get the evolution table
        stream.Position = __evoTableOffset;
        var length = stream.ReadInt32();
        var buffer = new byte[length];
        stream.ReadExactly(buffer);

        // put the data onto the pokemon
        foreach (var pokemon in Enumerate())
        {
            int minEvo = pokemon.MinEvolutionTableEntry;
            int maxEvo = pokemon.MaxEvolutionTableEntry;
            if (minEvo != __defaultEvoId && maxEvo != __defaultEvoId)
            {
                for (int i = minEvo; i <= maxEvo; i++)
                {
                    if (i < buffer.Length) // Prevent crash if v6.0 broke mods. Evolutions will be messed up but won't crash.
                    {
                        pokemon.Evolutions.Add((PokemonId)buffer[i]);
                    }
                }
            }
        }
    }

    public override void PreSave(Stream stream)
    {
        // Generate evolution table
        _evoTable.Clear();
        foreach (var pokemon in Enumerate())
        {
            if (pokemon.Evolutions.Count == 0)
            {
                pokemon.MinEvolutionTableEntry = __defaultEvoId;
                pokemon.MaxEvolutionTableEntry = __defaultEvoId;
            }
            else
            {
                pokemon.MinEvolutionTableEntry = _evoTable.Count;
                foreach (var evo in pokemon.Evolutions)
                {
                    _evoTable.Add(evo);
                }
                pokemon.MaxEvolutionTableEntry = _evoTable.Count - 1;
            }
        }

        // Generate name order index
        int c = 0;
        // Japanese string comparer works correctly for english too
        StringComparer cmp = StringComparer.Create(new CultureInfo("ja-JP"), ignoreCase: true);
        foreach (var pokemon in Enumerate().OrderBy(x => x.Name, cmp))
        {
            pokemon.NameOrderIndex = c;
            c++;
        }
    }

    private readonly List<PokemonId> _evoTable = [];

    public override void PostSave(Stream stream)
    {
        // write the evolution table
        stream.Position = __evoTableOffset;
        int length = _evoTable.Count;
        stream.WriteInt32(length);
        stream.Write(_evoTable.Select(i => (byte)i).ToArray());
        // this is padded to be divisible by 4
        if (length % 4 != 0)
        {
            stream.Pad(4 - (length % 4));
        }
        stream.SetLength(stream.Position);
        _evoTable.Clear();
    }
}