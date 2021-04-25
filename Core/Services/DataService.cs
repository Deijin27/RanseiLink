using Core.Enums;
using Core.Models;
using Ransei.Nds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Services
{
    public class DataService : IDataService<PokemonId, Pokemon>, IDataService<MoveId, Move>, IDataService<AbilityId, Ability>, IDataService<SaihaiId, Saihai>
    {
        readonly string DataFolder;
        const string PokemonFile = "Pokemon.dat";
        const string MoveFile = "Waza.dat";
        const string AbilityFile = "Tokusei.dat";
        const string SaihaiFile = "Saihai.dat";

        const string PokemonRomPath = "/data/Pokemon.dat";
        const string MoveRomPath = "/data/Waza.dat";
        const string AbilityRomPath = "/data/Tokusei.dat";
        const string SaihaiRomPath = "/data/Saihai.dat";

        public DataService()
        {
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ransei");
            Directory.CreateDirectory(DataFolder);
            foreach (string file in new string[] { PokemonFile, MoveFile, AbilityFile, SaihaiFile })
            {
                string p = Path.Combine(DataFolder, file);
                if (!File.Exists(p))
                {
                    File.WriteAllBytes(p, new byte[20000]); // Temporary just to make sure it doesn't crash. Need to find a better way to deal with situations 
                                                            // where no rom has been loaded yet.
                }
            }
        }

        public void LoadRom(string path)
        {
            using (var stream = new BinaryReader(File.OpenRead(path)))
            {
                Nds.CopyExtractFile(stream, PokemonRomPath, Path.Combine(DataFolder, PokemonFile));
                Nds.CopyExtractFile(stream, MoveRomPath, Path.Combine(DataFolder, MoveFile));
                Nds.CopyExtractFile(stream, AbilityRomPath, Path.Combine(DataFolder, AbilityFile));
                Nds.CopyExtractFile(stream, SaihaiRomPath, Path.Combine(DataFolder, SaihaiFile));
            }
        }

        public void CommitToRom(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                Nds.InsertFixedLengthFile(stream, PokemonRomPath, Path.Combine(DataFolder, PokemonFile));
                Nds.InsertFixedLengthFile(stream, MoveRomPath, Path.Combine(DataFolder, MoveFile));
                Nds.InsertFixedLengthFile(stream, AbilityRomPath, Path.Combine(DataFolder, AbilityFile));
                Nds.InsertFixedLengthFile(stream, SaihaiRomPath, Path.Combine(DataFolder, SaihaiFile));
            }
        }

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
                file.BaseStream.Position = (int)id * Move.DataLength;
                file.Write(model.Data);
            }
        }

        public Ability Retrieve(AbilityId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, AbilityFile))))
            {
                file.BaseStream.Position = (int)id * Ability.DataLength;
                return new Ability(file.ReadBytes(Ability.DataLength));
            }
        }

        public void Save(AbilityId id, Ability model)
        {
            using (var file = new BinaryWriter(File.OpenRead(Path.Combine(DataFolder, AbilityFile))))
            {
                file.BaseStream.Position = (int)id * Ability.DataLength;
                file.Write(model.Data);
            }
        }

        public Saihai Retrieve(SaihaiId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, SaihaiFile))))
            {
                file.BaseStream.Position = (int)id * Saihai.DataLength;
                return new Saihai(file.ReadBytes(Saihai.DataLength));
            }
        }

        public void Save(SaihaiId id, Saihai model)
        {
            using (var file = new BinaryWriter(File.OpenRead(Path.Combine(DataFolder, SaihaiFile))))
            {
                file.BaseStream.Position = (int)id * Saihai.DataLength;
                file.Write(model.Data);
            }
        }
    }
}
