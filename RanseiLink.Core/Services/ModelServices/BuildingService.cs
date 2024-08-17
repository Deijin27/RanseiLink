using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public interface IBuildingService : IModelService<Building>
{
}

public class BuildingService : BaseDataModelService<Building>, IBuildingService
{
    private BuildingService(string BuildingDatFile, ConquestGameCode culture) 
        : base(BuildingDatFile, 0, 118, () => new Building(culture), 119) 
    {
    }

    public Building Retrieve(BuildingId id) => Retrieve((int)id);

    public BuildingService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BuildingRomPath), mod.GameCode) { }

    public override string IdToName(int id)
    {
        return ((BuildingId)id).ToString();
    }
} 