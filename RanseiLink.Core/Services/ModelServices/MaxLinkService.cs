using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMaxLinkService : IModelService<MaxLink>
    {
    }

    public class MaxLinkService : BaseModelService<MaxLink>, IMaxLinkService
    {
        public MaxLinkService(string MaxLinkDatFile) : base(MaxLinkDatFile, 0, 251) { }

        public MaxLinkService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MaxLinkRomPath)) { }

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new MaxLink(br.ReadBytes(MaxLink.DataLength)));
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
            return ((WarriorId)id).ToString();
        }
    } 
}