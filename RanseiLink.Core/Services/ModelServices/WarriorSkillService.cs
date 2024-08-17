using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IWarriorSkillService : IModelService<WarriorSkill>
    {
    }

    public class WarriorSkillService : BaseDataModelService<WarriorSkill>, IWarriorSkillService
    {
        private WarriorSkillService(string WarriorSkillDatFile, ConquestGameCode culture = ConquestGameCode.VPYT) 
            : base(WarriorSkillDatFile, 0, 72, () => new WarriorSkill(culture)) 
        {
        }

        public WarriorSkillService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.WarriorSkillRomPath), mod.GameCode) { }

        public WarriorSkill Retrieve(WarriorSkillId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            return Retrieve(id).Name;
        }
    } 
}