using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMoveAnimationService : IModelService<MoveAnimation>
    {
    }

    public class MoveAnimationService : BaseModelService<MoveAnimation>, IMoveAnimationService
    {
        public MoveAnimationService(string MoveAnimationDatFile) : base(MoveAnimationDatFile, 0, 254) { }

        public MoveAnimationService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MoveAnimationRomPath)) { }

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new MoveAnimation(br.ReadBytes(MoveAnimation.DataLength)));
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
            return ((MoveAnimationId)id).ToString();
        }
    } 
}