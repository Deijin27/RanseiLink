using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class GimmickService : BaseModelService, IModelDataService<GimmickId, IGimmick>
    {
        public GimmickService(ModInfo mod) : base(mod, Constants.GimmickRomPath, Gimmick.DataLength) { }

        public IGimmick Retrieve(GimmickId id)
        {
            return new Gimmick(RetrieveData((int)id));
        }

        public void Save(GimmickId id, IGimmick model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
