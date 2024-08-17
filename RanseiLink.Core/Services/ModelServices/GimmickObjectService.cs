using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IGimmickObjectService : IModelService<GimmickObject>
    {
    }

    public class GimmickObjectService : BaseNewableDataModelService<GimmickObject>, IGimmickObjectService
    {
        private GimmickObjectService(string GimmickObjectDatFile) : base(GimmickObjectDatFile, 0, 99) { }

        public GimmickObjectService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.GimmickObjectRomPath)) { }

        public GimmickObject Retrieve(GimmickObjectId id) => Retrieve((int)id);

        public override string IdToName(int id)
        {
            return ((GimmickObjectId)id).ToString();
        }
    } 
}