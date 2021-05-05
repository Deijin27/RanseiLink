using Core.Enums;
using Core.Models;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IDataService : 
        IModelDataService<PokemonId, Pokemon>,
        IModelDataService<MoveId, Move>,
        IModelDataService<AbilityId, Ability>,
        IModelDataService<SaihaiId, Saihai>,
        IModelDataService<GimmickId, Gimmick>,
        IModelDataService<BuildingId, Building>
    {
        Dictionary<PokemonId, Pokemon> AllPokemon();
        void CommitToRom(string path);
        void LoadRom(string path);
    }
}