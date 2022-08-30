using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IPokemonService : IModelService<Pokemon>
    {
    }

    public class PokemonService : BaseModelService<Pokemon>, IPokemonService
    {
        private const int _defaultEvoId = 1400;
        private const long _evoTableOffset = 0x25C0;

        public PokemonService(string pokemonDatFile) : base(pokemonDatFile, 0, 199, 511) { }
        public PokemonService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.PokemonRomPath)) { }

        public Pokemon Retrieve(PokemonId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                // get the evolution table
                br.BaseStream.Position = _evoTableOffset;
                int length = br.ReadInt32();
                List<PokemonId> evoTable = br.ReadBytes(length).Select(i => (PokemonId)i).ToList();

                // get the pokemon, filling in their evolutions as you go
                br.BaseStream.Position = 0;
                for (int id = _minId; id <= _maxId; id++)
                {
                    var data = br.ReadBytes(Pokemon.DataLength);
                    var pokemon = new Pokemon(data);
                    _cache.Add(pokemon);
                    int minEvo = (int)pokemon.MinEvolutionTableEntry;
                    int maxEvo = (int)pokemon.MaxEvolutionTableEntry;
                    if (minEvo != _defaultEvoId && maxEvo != _defaultEvoId)
                    {
                        for (int i = minEvo; i <= maxEvo; i++)
                        {
                            pokemon.Evolutions.Add(evoTable[i]);
                        }
                    }
                }
            }
        }

        public override void Save()
        {
            // update evolutions in pokemon models and build new evolution table
            List<PokemonId> evoTable = new List<PokemonId>();
            using (var bw = new BinaryWriter(File.OpenWrite(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    Pokemon pokemon = _cache[id];
                    if (pokemon.Evolutions.Count == 0)
                    {
                        pokemon.MinEvolutionTableEntry = _defaultEvoId;
                        pokemon.MaxEvolutionTableEntry = _defaultEvoId;
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
                    bw.Write(pokemon.Data);
                }
                // write the evolution table
                bw.BaseStream.Position = _evoTableOffset;
                int length = evoTable.Count;
                bw.Write(length);
                bw.Write(evoTable.Select(i => (byte)i).ToArray());
                // this is padded to be divisible by 4
                if (length % 4 != 0)
                {
                    bw.Pad(4 - (length % 4));
                }
                bw.BaseStream.SetLength(bw.BaseStream.Position);
            }
        }

        public override string IdToName(int id)
        {
            if (!ValidateId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return _cache[id].Name;
        }
    }
}