using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;


public interface IGimmickService : IModelService<Gimmick>
{
}

public class GimmickService : BaseDataModelService<Gimmick>, IGimmickService
{
    private GimmickService(string GimmickDatFile, ConquestGameCode culture = ConquestGameCode.VPYT) 
        : base(GimmickDatFile, 0, 147, () => new Gimmick(culture)) 
    {
    }

    public GimmickService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.GimmickRomPath), mod.GameCode) { }

    public Gimmick Retrieve(GimmickId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return ((GimmickId)id).ToString();
    }
} 