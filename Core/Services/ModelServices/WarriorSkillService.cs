using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class WarriorSkillService : BaseModelService, IModelDataService<WarriorSkillId, IWarriorSkill>
    {
        public WarriorSkillService(ModInfo mod) : base(mod, Constants.WarriorSkillRomPath, WarriorSkill.DataLength) { }

        public IWarriorSkill Retrieve(WarriorSkillId id)
        {
            return new WarriorSkill(RetrieveData((int)id));
        }

        public void Save(WarriorSkillId id, IWarriorSkill model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
