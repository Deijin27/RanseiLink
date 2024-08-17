using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMoveRangeService : IModelService<MoveRange>
    {
    }

    public class MoveRangeService : BaseNewableDataModelService<MoveRange>, IMoveRangeService
    {
        private MoveRangeService(string MoveRangeDatFile) : base(MoveRangeDatFile, 0, 29) { }

        public MoveRangeService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MoveRangeRomPath)) { }

        public MoveRange Retrieve(MoveRangeId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            return ((MoveRangeId)id).ToString();
        }
    } 
}