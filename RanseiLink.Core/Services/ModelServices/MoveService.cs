using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
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
