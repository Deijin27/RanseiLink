using Core.Models;
using Core.Models.Interfaces;
using System.IO;

namespace Core.Services.ModelServices
{
    public interface IEvolutionTableService : IModelDataService<IEvolutionTable>
    {
        IDisposableEvolutionTableService Disposable();
    }

    public interface IDisposableEvolutionTableService : IDisposableModelDataService<IEvolutionTable>
    {
    }

    public class EvolutionTableService : IEvolutionTableService
    {
        private readonly ModInfo Mod;
        private readonly string CurrentModFolder;
        public EvolutionTableService(ModInfo mod)
        {
            Mod = mod;
            CurrentModFolder = mod.FolderPath;
        }

        public IDisposableEvolutionTableService Disposable()
        {
            return new DisposableEvolutionTableService(Mod);
        }

        public IEvolutionTable Retrieve()
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(CurrentModFolder, Constants.PokemonRomPath))))
            {
                file.BaseStream.Position = 0x25C4;
                return new EvolutionTable(file.ReadBytes(EvolutionTable.DataLength));
            }
        }

        public void Save(IEvolutionTable model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(CurrentModFolder, Constants.PokemonRomPath))))
            {
                file.BaseStream.Position = 0x25C4;
                file.Write(model.Data);
            }
        }
    }

    public class DisposableEvolutionTableService : IDisposableEvolutionTableService
    {
        private readonly Stream Stream;
        public DisposableEvolutionTableService(ModInfo mod)
        {
            Stream = File.Open(Path.Combine(mod.FolderPath, Constants.PokemonRomPath), FileMode.Open, FileAccess.ReadWrite);
        }

        public void Dispose()
        {
            Stream.Close();
        }

        public IEvolutionTable Retrieve()
        {
            Stream.Position = 0x25C4;
            byte[] buffer = new byte[EvolutionTable.DataLength];
            Stream.Read(buffer, 0, EvolutionTable.DataLength);
            return new EvolutionTable(buffer);
        }

        public void Save(IEvolutionTable model)
        {
            Stream.Position = 0x25C4;
            Stream.Write(model.Data, 0, EvolutionTable.DataLength);
        }
    }
}
