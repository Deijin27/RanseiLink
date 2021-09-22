using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class MoveRangeService : BaseModelService, IModelDataService<MoveRangeId, IMoveRange>
    {
        public MoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, MoveRange.DataLength) { }

        public IMoveRange Retrieve(MoveRangeId id)
        {
            return new MoveRange(RetrieveData((int)id));
        }

        public void Save(MoveRangeId id, IMoveRange model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
