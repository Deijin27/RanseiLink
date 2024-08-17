using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices;

public interface IAbilityService : IModelService<Ability>
{
}

public class AbilityService : BaseNewableDataModelService<Ability>, IAbilityService
{
    private AbilityService(string abilityDatFile) : base(abilityDatFile, 0, 127, 128) { }

    public AbilityService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.AbilityRomPath)) { }

    public Ability Retrieve(AbilityId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
}