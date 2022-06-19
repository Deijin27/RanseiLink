using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices
{

    public interface IGimmickService : IModelService<Gimmick>
    {
    }

    public class GimmickService : BaseModelService<Gimmick>, IGimmickService
    {
        public GimmickService(string GimmickDatFile) : base(GimmickDatFile, 0, 147) { }

        public GimmickService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.GimmickRomPath)) { }

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Gimmick(br.ReadBytes(Gimmick.DataLength)));
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
            return ((GimmickId)id).ToString();
        }
    } 
}