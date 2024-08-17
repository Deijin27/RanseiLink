using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IKingdomService : IModelService<Kingdom>
{
}

public class KingdomService : BaseDataModelService<Kingdom>, IKingdomService
{
    private KingdomService(string KingdomDatFile, ConquestGameCode culture) 
        : base(KingdomDatFile, 0, 16, () => new Kingdom(culture), 17) 
    {
    }

    public KingdomService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.KingdomRomPath), mod.GameCode) { }

    public Kingdom Retrieve(KingdomId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
} 