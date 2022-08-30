using RanseiLink.Core.Models;
using System.IO;
using System;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IEpisodeService : IModelService<Episode>
    {
    }

    public class EpisodeService : BaseModelService<Episode>, IEpisodeService
    {
        public EpisodeService(string abilityDatFile) : base(abilityDatFile, 0, 37, 511) { }

        public EpisodeService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.EpisodeRomPath)) { }

        public Episode Retrieve(EpisodeId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Episode(br.ReadBytes(Episode.DataLength)));
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
            return ((EpisodeId)id).ToString();
        }
    }
}