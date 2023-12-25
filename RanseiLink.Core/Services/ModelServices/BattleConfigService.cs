using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IBattleConfigService : IModelService<BattleConfig>
    {
    }

    public class BattleConfigService : BaseModelService<BattleConfig>, IBattleConfigService
    {
        private BattleConfigService(string BattleConfigDatFile) : base(BattleConfigDatFile, 0, 46, 47) { }

        public BattleConfigService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BattleConfigRomPath)) { }

        public BattleConfig Retrieve(BattleConfigId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new BattleConfig(br.ReadBytes(BattleConfig.DataLength)));
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
            return ((BattleConfigId)id).ToString();
        }
    }
}