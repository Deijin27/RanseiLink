using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioAppearPokemonService : IModelService<ScenarioAppearPokemon>
    {
    }

    public class ScenarioAppearPokemonService : BaseModelService<ScenarioAppearPokemon>, IScenarioAppearPokemonService
    {
        public ScenarioAppearPokemonService(ModInfo mod) : base(mod.FolderPath, 0, 10)
        {
        }

        public ScenarioAppearPokemon Retrieve(ScenarioId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var br = new BinaryReader(File.OpenRead(Path.Combine(_dataFile, Constants.ScenarioAppearPokemonPathFromId(id)))))
                {
                    _cache.Add(new ScenarioAppearPokemon(br.ReadBytes(ScenarioAppearPokemon.DataLength)));
                }
            }
        }

        public override void Save()
        {
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var bw = new BinaryWriter(File.OpenWrite(Path.Combine(_dataFile, Constants.ScenarioAppearPokemonPathFromId(id)))))
                {
                    bw.Write(_cache[id].Data);
                } 
            }
        }

        public override string IdToName(int id)
        {
            if (!ValidateId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return ((ScenarioId)id).ToString();
        }
    } 
}
