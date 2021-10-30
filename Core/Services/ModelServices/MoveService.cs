using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IMoveService : IModelDataService<MoveId, IMove>
    {
        IDisposableMoveService Disposable();
    }

    public interface IDisposableMoveService : IDisposableModelDataService<MoveId, IMove>
    {
    }

    public class MoveService : BaseModelService, IMoveService
    {
        public MoveService(ModInfo mod) : base(mod, Constants.MoveRomPath, Move.DataLength, 142) { }

        public IDisposableMoveService Disposable()
        {
            return new DisposableMoveService(Mod);
        }

        public IMove Retrieve(MoveId id)
        {
            return new Move(RetrieveData((int)id));
        }

        public void Save(MoveId id, IMove model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableMoveService : BaseDisposableModelService, IDisposableMoveService
    {
        public DisposableMoveService(ModInfo mod) : base(mod, Constants.MoveRomPath, Move.DataLength, 142) { }

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
