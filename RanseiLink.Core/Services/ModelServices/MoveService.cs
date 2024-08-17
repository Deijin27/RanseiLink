using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IMoveService : IModelService<Move>
{
}

public class MoveService : BaseNewableDataModelService<Move>, IMoveService
{
    private MoveService(string MoveDatFile) : base(MoveDatFile, 0, 142) { }

    public MoveService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MoveRomPath)) { }

    public Move Retrieve(MoveId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
} 