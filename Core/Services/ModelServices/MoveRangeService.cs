using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IMoveRangeService : IModelDataService<MoveRangeId, IMoveRange>
    {
        IDisposableMoveRangeService Disposable();
    }

    public interface IDisposableMoveRangeService : IDisposableModelDataService<MoveRangeId, IMoveRange>
    {
    }

    public class MoveRangeService : BaseModelService, IMoveRangeService
    {
        public MoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, MoveRange.DataLength) { }

        public IDisposableMoveRangeService Disposable()
        {
            return new DisposableMoveRangeService(Mod);
        }

        public IMoveRange Retrieve(MoveRangeId id)
        {
            return new MoveRange(RetrieveData((int)id));
        }

        public void Save(MoveRangeId id, IMoveRange model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableMoveRangeService : BaseDisposableModelService, IDisposableMoveRangeService
    {
        public DisposableMoveRangeService(ModInfo mod) : base(mod, Constants.MoveRangeRomPath, MoveRange.DataLength) { }

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
