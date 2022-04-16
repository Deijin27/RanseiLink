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
        public KingdomService(string KingdomDatFile) : base(KingdomDatFile, 0, 16, 17) { }

        public KingdomService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.KingdomRomPath)) { }

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Kingdom(br.ReadBytes(Kingdom.DataLength)));
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