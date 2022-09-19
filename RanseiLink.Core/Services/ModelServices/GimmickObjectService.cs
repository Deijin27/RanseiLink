using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IGimmickObjectService : IModelService<GimmickObject>
    {
    }

    public class GimmickObjectService : BaseModelService<GimmickObject>, IGimmickObjectService
    {
        private GimmickObjectService(string GimmickObjectDatFile) : base(GimmickObjectDatFile, 0, 99) { }

        public GimmickObjectService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.GimmickObjectRomPath)) { }

        public GimmickObject Retrieve(GimmickObjectId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new GimmickObject(br.ReadBytes(GimmickObject.DataLength)));
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
            return ((GimmickObjectId)id).ToString();
        }
    } 
}