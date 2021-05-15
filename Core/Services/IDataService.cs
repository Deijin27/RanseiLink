using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IDataService : 
        IModelDataService<PokemonId, IPokemon>,
        IModelDataService<MoveId, Move>,
        IModelDataService<AbilityId, Ability>,
        IModelDataService<SaihaiId, Saihai>,
        IModelDataService<GimmickId, Gimmick>,
        IModelDataService<BuildingId, Building>
    {
        Dictionary<PokemonId, IPokemon> AllPokemon();
        void CommitToRom(string path);
        void LoadRom(string path);
    }
}