﻿using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IAbilityService : IModelDataService<AbilityId, IAbility>
    {
        IDisposableAbilityService Disposable();
    }

    public interface IDisposableAbilityService : IDisposableModelDataService<AbilityId, IAbility>
    {
    }

    public class AbilityService : BaseModelService, IAbilityService
    {
        public AbilityService(ModInfo mod) : base(mod, Constants.AbilityRomPath, Ability.DataLength, 127) { }

        public IDisposableAbilityService Disposable()
        {
            return new DisposableAbilityService(Mod);
        }

        public IAbility Retrieve(AbilityId id)
        {
            return new Ability(RetrieveData((int)id));
        }

        public void Save(AbilityId id, IAbility model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableAbilityService : BaseDisposableModelService, IDisposableAbilityService
    {
        public DisposableAbilityService(ModInfo mod) : base(mod, Constants.AbilityRomPath, Ability.DataLength, 127) { }

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