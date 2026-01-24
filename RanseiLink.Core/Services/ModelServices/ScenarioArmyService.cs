using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IChildScenarioArmyService : IModelService<ScenarioArmy>
{
}

public interface IScenarioArmyService : IModelService<IChildScenarioArmyService>
{

}

public class ScenarioArmyService : BaseModelService<IChildScenarioArmyService>, IScenarioArmyService
{
    public ScenarioArmyService(ModInfo mod) : base(string.Empty, 0, 10)
    {
        for (int i = _minId; i <= _maxId; i++)
        {
            _cache.Add(new ChildScenarioArmyService(Path.Combine(mod.FolderPath, Constants.ScenarioArmyPathFromId(i))));
        }
    }

    public IChildScenarioArmyService Retrieve(ScenarioId id) => Retrieve((int)id);

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

public class ChildScenarioArmyService : BaseNewableDataModelService<ScenarioArmy>, IChildScenarioArmyService
{
    public ChildScenarioArmyService(string scenarioArmyDatFile) : base(scenarioArmyDatFile, 0, 16)
    {
    }

    public override string IdToName(int id)
    {
        return id.ToString();
    }
}