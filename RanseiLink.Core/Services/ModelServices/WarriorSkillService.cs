using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IWarriorSkillService : IModelDataService<WarriorSkillId, IWarriorSkill>
{
    IDisposableWarriorSkillService Disposable();
}

public interface IDisposableWarriorSkillService : IDisposableModelDataService<WarriorSkillId, IWarriorSkill>
{
}

public class WarriorSkillService : BaseModelService, IWarriorSkillService
{
    public WarriorSkillService(ModInfo mod) : base(mod, Constants.WarriorSkillRomPath, WarriorSkill.DataLength, 72) { }

    public IDisposableWarriorSkillService Disposable()
    {
        return new DisposableWarriorSkillService(Mod);
    }

    public IWarriorSkill Retrieve(WarriorSkillId id)
    {
        return new WarriorSkill(RetrieveData((int)id));
    }

    public void Save(WarriorSkillId id, IWarriorSkill model)
    {
        SaveData((int)id, model.Data);
    }
}

public class DisposableWarriorSkillService : BaseDisposableModelService, IDisposableWarriorSkillService
{
    public DisposableWarriorSkillService(ModInfo mod) : base(mod, Constants.WarriorSkillRomPath, WarriorSkill.DataLength, 72) { }

    public IWarriorSkill Retrieve(WarriorSkillId id)
    {
        return new WarriorSkill(RetrieveData((int)id));
    }

    public void Save(WarriorSkillId id, IWarriorSkill model)
    {
        SaveData((int)id, model.Data);
    }
}
