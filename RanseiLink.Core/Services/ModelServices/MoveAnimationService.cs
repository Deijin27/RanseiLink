using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMoveAnimationService : IModelService<MoveAnimation>
    {
    }

    public class MoveAnimationService : BaseNewableDataModelService<MoveAnimation>, IMoveAnimationService
    {
        private MoveAnimationService(string MoveAnimationDatFile) : base(MoveAnimationDatFile, 0, 254) { }

        public MoveAnimationService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MoveAnimationRomPath)) { }

        public MoveAnimation Retrieve(MoveAnimationId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            return ((MoveAnimationId)id).ToString();
        }
    } 
}