using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;
using System.IO;

namespace Core.Services.ModelServices
{
    public interface IPokemonService : IModelDataService<PokemonId, IPokemon>
    {
        IDisposablePokemonService Disposable();

        IEvolutionTable RetrieveEvolutionTable();

        void SaveEvolutionTable(IEvolutionTable model);
    }

    public interface IDisposablePokemonService : IDisposableModelDataService<PokemonId, IPokemon>
    {
        IEvolutionTable RetrieveEvolutionTable();

        void SaveEvolutionTable(IEvolutionTable model);
    }

    public class PokemonService : BaseModelService, IPokemonService
    {
        public PokemonService(ModInfo mod) : base(mod, Constants.PokemonRomPath, Pokemon.DataLength) { }

        public IDisposablePokemonService Disposable() => new DisposablePokemonService(Mod);

        public IPokemon Retrieve(PokemonId id)
        {
            return new Pokemon(RetrieveData((int)id));
        }

        public void Save(PokemonId id, IPokemon model)
        {
            SaveData((int)id, model.Data);
        }

        public IEvolutionTable RetrieveEvolutionTable()
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(CurrentModFolder, Constants.PokemonRomPath))))
            {
                file.BaseStream.Position = 0x25C4;
                return new EvolutionTable(file.ReadBytes(EvolutionTable.DataLength));
            }
        }

        public void SaveEvolutionTable(IEvolutionTable model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(CurrentModFolder, Constants.PokemonRomPath))))
            {
                file.BaseStream.Position = 0x25C4;
                file.Write(model.Data);
            }
        }
    }

    public class DisposablePokemonService : BaseDisposableModelService, IDisposablePokemonService
    {
        public DisposablePokemonService(ModInfo mod) : base(mod, Constants.PokemonRomPath, Pokemon.DataLength) { }

        public IPokemon Retrieve(PokemonId id)
        {
            return new Pokemon(RetrieveData((int)id));
        }

        public void Save(PokemonId id, IPokemon model)
        {
            SaveData((int)id, model.Data);
        }

        public IEvolutionTable RetrieveEvolutionTable()
        {
            stream.Position = 0x25C4;
            byte[] buffer = new byte[EvolutionTable.DataLength];
            stream.Read(buffer, 0, EvolutionTable.DataLength);
            return new EvolutionTable(buffer);
        }

        public void SaveEvolutionTable(IEvolutionTable model)
        {
            stream.Position = 0x25C4;
            stream.Write(model.Data, 0, EvolutionTable.DataLength);
        }
    }
}
