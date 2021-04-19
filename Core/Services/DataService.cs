using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Services
{
    public class DataService : IDataService<PokemonId, Pokemon>, IDataService<MoveId, Move>
    {
        const string DataFolder = @"C:\Users\Mia\Desktop\PokemonRomEditing\Conquest";
        const string PokemonFile = "Pokemon.dat";
        const string MoveFile = "Waza.dat";

        public Pokemon Retrieve(PokemonId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, PokemonFile))))
            {
                file.BaseStream.Position = (int)id * Pokemon.DataLength;
                return new Pokemon(file.ReadBytes(Pokemon.DataLength));
            }
        }

        public void Save(PokemonId id, Pokemon model)
        {
            using (var file = new BinaryWriter(File.OpenRead(Path.Combine(DataFolder, PokemonFile))))
            {
                file.BaseStream.Position = (int)id * Pokemon.DataLength;
                file.Write(model.Data);
            }
        }

        public Move Retrieve(MoveId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, MoveFile))))
            {
                file.BaseStream.Position = (int)id * Move.DataLength;
                return new Move(file.ReadBytes(Move.DataLength));
            }
        }

        public void Save(MoveId id, Move model)
        {
            using (var file = new BinaryWriter(File.OpenRead(Path.Combine(DataFolder, MoveFile))))
            {
                file.BaseStream.Position = (int)id * Pokemon.DataLength;
                file.Write(model.Data);
            }
        }
    }
}
