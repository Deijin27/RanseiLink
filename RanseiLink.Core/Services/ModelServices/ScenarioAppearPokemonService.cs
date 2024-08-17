using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IScenarioAppearPokemonService : IModelService<ScenarioAppearPokemon>
    {
    }

    public class ScenarioAppearPokemonService : BaseScenarioModelService<ScenarioAppearPokemon>, IScenarioAppearPokemonService
    {
        public ScenarioAppearPokemonService(ModInfo mod) : base(mod.FolderPath, 0, 10)
        {
        }

        public ScenarioAppearPokemon Retrieve(ScenarioId id) => Retrieve((int)id);

        protected override string IdToRelativePath(int id)
        {
            return Constants.ScenarioAppearPokemonPathFromId(id);
        }

        public override string IdToName(int id)
        {
            return ((ScenarioId)id).ToString();
        }
    } 
}
