using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IKingdomService : IModelService<Kingdom>
    {
    }

    public class KingdomService : BaseModelService<Kingdom>, IKingdomService
    {
        private readonly ConquestGameCode _culture;
        private KingdomService(string KingdomDatFile, ConquestGameCode culture = ConquestGameCode.VPYT) : base(KingdomDatFile, 0, 16, 17, delayReload:true) 
        {
            _culture = culture;
            Reload();
        }

        public KingdomService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.KingdomRomPath), mod.GameCode) { }

        public Kingdom Retrieve(KingdomId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Kingdom(br.ReadBytes(Kingdom.DataLength(_culture)), _culture));
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