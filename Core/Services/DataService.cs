using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Services
{
    public class DataService : IDataService
    {
        readonly string DataFolder;
        const string PokemonFile = "Pokemon.dat";
        const string MoveFile = "Waza.dat";
        const string AbilityFile = "Tokusei.dat";
        const string SaihaiFile = "Saihai.dat";
        const string GimmickFile = "Gimmick.dat";
        const string BuildingFile = "Building.dat";

        const string PokemonRomPath = "/data/Pokemon.dat";
        const string MoveRomPath = "/data/Waza.dat";
        const string AbilityRomPath = "/data/Tokusei.dat";
        const string SaihaiRomPath = "/data/Saihai.dat";
        const string GimmickRomPath = "/data/Gimmick.dat";
        const string BuildingRomPath = "/data/Building.dat";

        public DataService()
        {
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ransei");
            Directory.CreateDirectory(DataFolder);
            foreach (string file in new string[] { PokemonFile, MoveFile, AbilityFile, SaihaiFile, GimmickFile, BuildingFile })
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
            using (var nds = new Nds.Nds(path))
            {
                nds.ExtractCopyOfFile(PokemonRomPath, Path.Combine(DataFolder, PokemonFile));
                nds.ExtractCopyOfFile(MoveRomPath, Path.Combine(DataFolder, MoveFile));
                nds.ExtractCopyOfFile(AbilityRomPath, Path.Combine(DataFolder, AbilityFile));
                nds.ExtractCopyOfFile(SaihaiRomPath, Path.Combine(DataFolder, SaihaiFile));
                nds.ExtractCopyOfFile(GimmickRomPath, Path.Combine(DataFolder, GimmickFile));
                nds.ExtractCopyOfFile(BuildingRomPath, Path.Combine(DataFolder, BuildingFile));
            }
        }

        public void CommitToRom(string path)
        {
            using (var nds = new Nds.Nds(path))
            {
                nds.InsertFixedLengthFile(PokemonRomPath, Path.Combine(DataFolder, PokemonFile));
                nds.InsertFixedLengthFile(MoveRomPath, Path.Combine(DataFolder, MoveFile));
                nds.InsertFixedLengthFile(AbilityRomPath, Path.Combine(DataFolder, AbilityFile));
                nds.InsertFixedLengthFile(SaihaiRomPath, Path.Combine(DataFolder, SaihaiFile));
                nds.InsertFixedLengthFile(GimmickRomPath, Path.Combine(DataFolder, GimmickFile));
                nds.InsertFixedLengthFile(BuildingRomPath, Path.Combine(DataFolder, BuildingFile));
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

        public Dictionary<PokemonId, Pokemon> AllPokemon()
        {
            var dict = new Dictionary<PokemonId, Pokemon>();

            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, PokemonFile))))
            {
                int count = (int)(file.BaseStream.Length / Pokemon.DataLength);
                for (int i = 0; i < count; i++)
                {
                    var pk = file.ReadBytes(Pokemon.DataLength);
                    dict[(PokemonId)i] = new Pokemon(pk);
                }
            }
            return dict;
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

        public Gimmick Retrieve(GimmickId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, GimmickFile))))
            {
                file.BaseStream.Position = (int)id * Gimmick.DataLength;
                return new Gimmick(file.ReadBytes(Gimmick.DataLength));
            }
        }

        public void Save(GimmickId id, Gimmick model)
        {
            using (var file = new BinaryWriter(File.OpenRead(Path.Combine(DataFolder, GimmickFile))))
            {
                file.BaseStream.Position = (int)id * Gimmick.DataLength;
                file.Write(model.Data);
            }
        }

        public Building Retrieve(BuildingId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, BuildingFile))))
            {
                file.BaseStream.Position = (int)id * Building.DataLength;
                return new Building(file.ReadBytes(Building.DataLength));
            }
        }

        public void Save(BuildingId id, Building model)
        {
            using (var file = new BinaryWriter(File.OpenRead(Path.Combine(DataFolder, BuildingFile))))
            {
                file.BaseStream.Position = (int)id * Building.DataLength;
                file.Write(model.Data);
            }
        }
    }
}
