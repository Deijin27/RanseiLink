using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IAbilityService : IModelService<Ability>
    {
    }

    public class AbilityService : BaseModelService<Ability>, IAbilityService
    {
        private AbilityService(string abilityDatFile) : base(abilityDatFile, 0, 127, 128) { }

        public AbilityService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.AbilityRomPath)) { }

        public Ability Retrieve(AbilityId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Ability(br.ReadBytes(Ability.DataLength)));
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