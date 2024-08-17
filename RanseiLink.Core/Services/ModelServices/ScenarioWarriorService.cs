using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

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

    public class ChildScenarioWarriorService : BaseNewableDataModelService<ScenarioWarrior>, IChildScenarioWarriorService
    {
        private readonly IBaseWarriorService _warriorService;
        public ChildScenarioWarriorService(string scenarioWarriorDatFile, IBaseWarriorService warriorService) : base(scenarioWarriorDatFile, 0, 209)
        {
            _warriorService = warriorService;
        }

        public override string IdToName(int id)
        {
            return _warriorService.IdToName((int)Retrieve(id).Warrior);
        }
    } 
}