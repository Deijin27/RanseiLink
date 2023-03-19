#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IChildScenarioWarriorService : IModelService<ScenarioWarrior>
    {
    }

    public interface IScenarioWarriorService : IModelService<IChildScenarioWarriorService>
    {

    }

    public class ScenarioWarriorService : BaseModelService<IChildScenarioWarriorService>, IScenarioWarriorService
    {
        public ScenarioWarriorService(ModInfo mod, IBaseWarriorService baseWarriorService) : base(string.Empty, 0, 10)
        {
            for (int i = _minId; i <= _maxId; i++)
            {
                _cache.Add(new ChildScenarioWarriorService(Path.Combine(mod.FolderPath, Constants.ScenarioWarriorPathFromId(i)), baseWarriorService));
            }
        }

        public IChildScenarioWarriorService Retrieve(ScenarioId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            if (!ValidateId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return ((ScenarioId)id).ToString();
        }

        public override void Reload()
        {
            foreach (var childService in Enumerate())
            {
                childService.Reload();
            }
        }

        public override void Save()
        {
            foreach (var childService in Enumerate())
            {
                childService.Save();
            }
        }
    }

    public class ChildScenarioWarriorService : BaseModelService<ScenarioWarrior>, IChildScenarioWarriorService
    {
        private readonly IBaseWarriorService _WarriorService;
        public ChildScenarioWarriorService(string scenarioWarriorDatFile, IBaseWarriorService WarriorService) : base(scenarioWarriorDatFile, 0, 209)
        {
            _WarriorService = WarriorService;
        }

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new ScenarioWarrior(br.ReadBytes(ScenarioWarrior.DataLength)));
                }
            }
                
        }

        public override void Save()
        {
            using (var bw = new BinaryWriter(File.OpenWrite(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
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
            return _WarriorService.IdToName((int)_cache[id].Warrior);
        }
    } 
}