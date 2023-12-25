using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IWarriorSkillService : IModelService<WarriorSkill>
    {
    }

    public class WarriorSkillService : BaseModelService<WarriorSkill>, IWarriorSkillService
    {
        private readonly ConquestGameCode _culture;
        private WarriorSkillService(string WarriorSkillDatFile, ConquestGameCode culture = ConquestGameCode.VPYT) : base(WarriorSkillDatFile, 0, 72, delayReload:true) 
        {
            _culture = culture;
            Reload();
        }

        public WarriorSkillService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.WarriorSkillRomPath), mod.GameCode) { }

        public WarriorSkill Retrieve(WarriorSkillId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new WarriorSkill(br.ReadBytes(WarriorSkill.DataLength(_culture)), _culture));
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
            return _cache[id].Name;
        }
    } 
}