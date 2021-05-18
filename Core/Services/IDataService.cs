using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IDataService : 
        IModelDataService<PokemonId, IPokemon>,
        IModelDataService<MoveId, IMove>,
        IModelDataService<AbilityId, IAbility>,
        IModelDataService<WarriorSkillId, IWarriorSkill>,
        IModelDataService<GimmickId, IGimmick>,
        IModelDataService<BuildingId, IBuilding>,
        IModelDataService<ItemId, IItem>
    {
        Dictionary<PokemonId, IPokemon> AllPokemon();
        void CommitToRom(string path);
        void LoadRom(string path);
    }
}