using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IEpisodeService : IModelService<Episode>
    {
    }

    public class EpisodeService : BaseNewableDataModelService<Episode>, IEpisodeService
    {
        private EpisodeService(string abilityDatFile) : base(abilityDatFile, 0, 37, 511) { }

        public EpisodeService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.EpisodeRomPath)) { }

        public Episode Retrieve(EpisodeId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            return ((EpisodeId)id).ToString();
        }
    }
}