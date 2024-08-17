using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IPokemonService : IModelService<Pokemon>
{
}

public class PokemonService : BaseNewableDataModelService<Pokemon>, IPokemonService
{
    private const int __defaultEvoId = 1400;
    private const long __evoTableOffset = 0x25C0;

    public static PokemonService Load(string pokemonDatFile) => new PokemonService(pokemonDatFile);
    private PokemonService(string pokemonDatFile) : base(pokemonDatFile, 0, 199, 511) { }
    public PokemonService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.PokemonRomPath)) { }

    public Pokemon Retrieve(PokemonId id) => Retrieve((int)id);

    public override void PostLoad(Stream stream)
    {
        // get the evolution table
        stream.Position = __evoTableOffset;
        var length = stream.ReadInt32();
        var buffer = new byte[length];
        stream.Read(buffer, 0, length);

        // put the data onto the pokemon
        foreach (var pokemon in Enumerate())
        {
            int minEvo = pokemon.MinEvolutionTableEntry;
            int maxEvo = pokemon.MaxEvolutionTableEntry;
            if (minEvo != __defaultEvoId && maxEvo != __defaultEvoId)
            {
                for (int i = minEvo; i <= maxEvo; i++)
                {
                    pokemon.Evolutions.Add((PokemonId)buffer[i]);
                }
            }
        }
    }

    public override void PostSave(Stream stream)
    {
        var evoTable = new List<PokemonId>();

        foreach (var pokemon in Enumerate())
        {
            if (pokemon.Evolutions.Count == 0)
            {
                pokemon.MinEvolutionTableEntry = __defaultEvoId;
                pokemon.MaxEvolutionTableEntry = __defaultEvoId;
            }
            else
            {
                pokemon.MinEvolutionTableEntry = evoTable.Count;
                foreach (var evo in pokemon.Evolutions)
                {
                    evoTable.Add(evo);
                }
                pokemon.MaxEvolutionTableEntry = evoTable.Count - 1;
            }
        }

        // write the evolution table
        stream.Position = __evoTableOffset;
        int length = evoTable.Count;
        stream.WriteInt32(length);
        stream.Write(evoTable.Select(i => (byte)i).ToArray());
        // this is padded to be divisible by 4
        if (length % 4 != 0)
        {
            stream.Pad(4 - (length % 4));
        }
        stream.SetLength(stream.Position);
    }

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
}