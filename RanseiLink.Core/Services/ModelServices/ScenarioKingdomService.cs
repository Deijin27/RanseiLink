using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioKingdomService : IModelService<ScenarioKingdom>
    {
    }

    public class ScenarioKingdomService : BaseModelService<ScenarioKingdom>, IScenarioKingdomService
    {
        public ScenarioKingdomService(ModInfo mod) : base(mod.FolderPath, 0, 10)
        {
        }

        public ScenarioKingdom Retrieve(ScenarioId id) => Retrieve((int)id);
        public override void Reload()
        {
            _cache.Clear();
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var br = new BinaryReader(File.OpenRead(Path.Combine(_dataFile, Constants.ScenarioKingdomPathFromId(id)))))
                {
                    _cache.Add(new ScenarioKingdom(br.ReadBytes(ScenarioKingdom.DataLength)));
                } 
            }
        }

        public override void Save()
        {
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var bw = new BinaryWriter(File.OpenWrite(Path.Combine(_dataFile, Constants.ScenarioKingdomPathFromId(id)))))
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