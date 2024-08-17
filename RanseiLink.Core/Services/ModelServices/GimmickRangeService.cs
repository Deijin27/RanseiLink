using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IGimmickRangeService : IModelService<MoveRange>
    {
    }

    public class GimmickRangeService : BaseNewableDataModelService<MoveRange>, IGimmickRangeService
    {
        private GimmickRangeService(string GimmickRangeDatFile) : base(GimmickRangeDatFile, 0, 29) { }

        public GimmickRangeService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.GimmickRangeRomPath)) { }

        public MoveRange Retrieve(GimmickRangeId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            return ((GimmickRangeId)id).ToString();
        }
    } 
}