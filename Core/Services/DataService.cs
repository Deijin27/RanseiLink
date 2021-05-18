using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
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
        const string WarriorSkillFile = "Saihai.dat";
        const string GimmickFile = "Gimmick.dat";
        const string BuildingFile = "Building.dat";
        const string ItemFile = "Item.dat";

        const string PokemonRomPath = "/data/Pokemon.dat";
        const string MoveRomPath = "/data/Waza.dat";
        const string AbilityRomPath = "/data/Tokusei.dat";
        const string WarriorSkillRomPath = "/data/Saihai.dat";
        const string GimmickRomPath = "/data/Gimmick.dat";
        const string BuildingRomPath = "/data/Building.dat";
        const string ItemRomPath = "/data/Item.dat";

        public DataService()
        {
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ransei");
            Init();
        }

        private void Init()
        {
            Directory.CreateDirectory(DataFolder);
            foreach (string file in new string[] { PokemonFile, MoveFile, AbilityFile, WarriorSkillFile, GimmickFile, BuildingFile, ItemFile})
            {
                string p = Path.Combine(DataFolder, file);
                if (!File.Exists(p))
                {
                    File.WriteAllBytes(p, new byte[20000]); // Temporary just to make sure it doesn't crash. Need to find a better way to deal with situations 
                                                            // where no rom has been loaded yet.
                }
            }
        }

        public DataService(string dataFolder)
        {
            DataFolder = dataFolder;
            Init();
        }

        public void LoadRom(string path)
        {
            using (var nds = new Nds.Nds(path))
            {
                nds.ExtractCopyOfFile(PokemonRomPath, DataFolder);
                nds.ExtractCopyOfFile(MoveRomPath, DataFolder);
                nds.ExtractCopyOfFile(AbilityRomPath, DataFolder);
                nds.ExtractCopyOfFile(WarriorSkillRomPath, DataFolder);
                nds.ExtractCopyOfFile(GimmickRomPath, DataFolder);
                nds.ExtractCopyOfFile(BuildingRomPath, DataFolder);
                nds.ExtractCopyOfFile(ItemRomPath, DataFolder);
            }
        }

        public void CommitToRom(string path)
        {
            using (var nds = new Nds.Nds(path))
            {
                nds.InsertFixedLengthFile(PokemonRomPath, Path.Combine(DataFolder, PokemonFile));
                nds.InsertFixedLengthFile(MoveRomPath, Path.Combine(DataFolder, MoveFile));
                nds.InsertFixedLengthFile(AbilityRomPath, Path.Combine(DataFolder, AbilityFile));
                nds.InsertFixedLengthFile(WarriorSkillRomPath, Path.Combine(DataFolder, WarriorSkillFile));
                nds.InsertFixedLengthFile(GimmickRomPath, Path.Combine(DataFolder, GimmickFile));
                nds.InsertFixedLengthFile(BuildingRomPath, Path.Combine(DataFolder, BuildingFile));
                nds.InsertFixedLengthFile(ItemRomPath, Path.Combine(DataFolder, ItemFile));
            }
        }


        public IPokemon Retrieve(PokemonId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, PokemonFile))))
            {
                file.BaseStream.Position = (int)id * Pokemon.DataLength;
                return new Pokemon(file.ReadBytes(Pokemon.DataLength));
            }
        }

        public void Save(PokemonId id, IPokemon model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, PokemonFile))))
            {
                file.BaseStream.Position = (int)id * Pokemon.DataLength;
                file.Write(model.Data);
            }
        }

        public Dictionary<PokemonId, IPokemon> AllPokemon()
        {
            var dict = new Dictionary<PokemonId, IPokemon>();

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

        public IMove Retrieve(MoveId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, MoveFile))))
            {
                file.BaseStream.Position = (int)id * Move.DataLength;
                return new Move(file.ReadBytes(Move.DataLength));
            }
        }

        public void Save(MoveId id, IMove model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, MoveFile))))
            {
                file.BaseStream.Position = (int)id * Move.DataLength;
                file.Write(model.Data);
            }
        }

        public IAbility Retrieve(AbilityId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, AbilityFile))))
            {
                file.BaseStream.Position = (int)id * Ability.DataLength;
                return new Ability(file.ReadBytes(Ability.DataLength));
            }
        }

        public void Save(AbilityId id, IAbility model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, AbilityFile))))
            {
                file.BaseStream.Position = (int)id * Ability.DataLength;
                file.Write(model.Data);
            }
        }

        public IWarriorSkill Retrieve(WarriorSkillId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, WarriorSkillFile))))
            {
                file.BaseStream.Position = (int)id * WarriorSkill.DataLength;
                return new WarriorSkill(file.ReadBytes(WarriorSkill.DataLength));
            }
        }

        public void Save(WarriorSkillId id, IWarriorSkill model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, WarriorSkillFile))))
            {
                file.BaseStream.Position = (int)id * WarriorSkill.DataLength;
                file.Write(model.Data);
            }
        }

        public IGimmick Retrieve(GimmickId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, GimmickFile))))
            {
                file.BaseStream.Position = (int)id * Gimmick.DataLength;
                return new Gimmick(file.ReadBytes(Gimmick.DataLength));
            }
        }

        public void Save(GimmickId id, IGimmick model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, GimmickFile))))
            {
                file.BaseStream.Position = (int)id * Gimmick.DataLength;
                file.Write(model.Data);
            }
        }

        public IBuilding Retrieve(BuildingId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, BuildingFile))))
            {
                file.BaseStream.Position = (int)id * Building.DataLength;
                return new Building(file.ReadBytes(Building.DataLength));
            }
        }

        public void Save(BuildingId id, IBuilding model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, BuildingFile))))
            {
                file.BaseStream.Position = (int)id * Building.DataLength;
                file.Write(model.Data);
            }
        }

        public IItem Retrieve(ItemId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(DataFolder, ItemFile))))
            {
                file.BaseStream.Position = (int)id * Item.DataLength;
                return new Item(file.ReadBytes(Item.DataLength));
            }
        }

        public void Save(ItemId id, IItem model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(DataFolder, ItemFile))))
            {
                file.BaseStream.Position = (int)id * Item.DataLength;
                file.Write(model.Data);
            }
        }
    }
}
