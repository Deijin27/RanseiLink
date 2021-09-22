using Core.Models;
using Core.Models.Interfaces;
using System.IO;

namespace Core.Services.ModelServices
{
    public class EvolutionTableService : IModelDataService<IEvolutionTable>
    {
        private readonly string CurrentModFolder;
        public EvolutionTableService(ModInfo mod)
        {
            CurrentModFolder = mod.FolderPath;
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
}
