using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioBuildingService : IModelService<ScenarioBuilding>
    {
    }

    public class ScenarioBuildingService : BaseModelService<ScenarioBuilding>, IScenarioBuildingService
    {
        public ScenarioBuildingService(ModInfo mod) : base(mod.FolderPath, 0, 10)
        {
        }

        public ScenarioBuilding Retrieve(ScenarioId id) => Retrieve((int)id);
        public override void Reload()
        {
            _cache.Clear();
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var br = new BinaryReader(File.OpenRead(Path.Combine(_dataFile, Constants.ScenarioBuildingPathFromId(id)))))
                {
                    _cache.Add(new ScenarioBuilding(br.ReadBytes(ScenarioBuilding.DataLength)));
                }
            }
        }

        public override void Save()
        {
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var bw = new BinaryWriter(File.OpenWrite(Path.Combine(_dataFile, Constants.ScenarioBuildingPathFromId(id)))))
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
