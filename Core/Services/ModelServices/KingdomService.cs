using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class KingdomService : BaseModelService, IModelDataService<KingdomId, IKingdom>
    {
        public KingdomService(ModInfo mod) : base(mod, Constants.KingdomRomPath, Kingdom.DataLength) { }

        public IKingdom Retrieve(KingdomId id)
        {
            return new Kingdom(RetrieveData((int)id));
        }

        public void Save(KingdomId id, IKingdom model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
