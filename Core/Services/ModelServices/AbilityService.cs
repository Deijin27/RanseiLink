using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class AbilityService : BaseModelService, IModelDataService<AbilityId, IAbility>
    {
        public AbilityService(ModInfo mod) : base(mod, Constants.AbilityRomPath, Ability.DataLength) { }

        public IAbility Retrieve(AbilityId id)
        {
            return new Ability(RetrieveData((int)id));
        }

        public void Save(AbilityId id, IAbility model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
