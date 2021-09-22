using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class MoveService : BaseModelService, IModelDataService<MoveId, IMove>
    {
        public MoveService(ModInfo mod) : base(mod, Constants.MoveRomPath, Move.DataLength) { }

        public IMove Retrieve(MoveId id)
        {
            return new Move(RetrieveData((int)id));
        }

        public void Save(MoveId id, IMove model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
