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
        private readonly ConquestGameCode _culture;
        public GimmickService(string GimmickDatFile, ConquestGameCode culture = ConquestGameCode.VPYT) : base(GimmickDatFile, 0, 147, delayReload:true) 
        {
            _culture = culture;
            Reload();
        }

        public GimmickService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.GimmickRomPath), mod.GameCode) { }

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Gimmick(br.ReadBytes(Gimmick.DataLength(_culture)), _culture));
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