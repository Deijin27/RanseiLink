using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IPokemonService : IModelDataService<PokemonId, IPokemon>
    {
        IDisposablePokemonService Disposable();
    }

    public interface IDisposablePokemonService : IDisposableModelDataService<PokemonId, IPokemon>
    {
    }

    public class PokemonService : BaseModelService, IPokemonService
    {
        public PokemonService(ModInfo mod) : base(mod, Constants.PokemonRomPath, Pokemon.DataLength) { }

        public IDisposablePokemonService Disposable() => new DisposablePokemonService(Mod);

        public IPokemon Retrieve(PokemonId id)
        {
            return new Pokemon(RetrieveData((int)id));
        }

        public void Save(PokemonId id, IPokemon model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposablePokemonService : BaseDisposableModelService, IDisposablePokemonService
    {
        public DisposablePokemonService(ModInfo mod) : base(mod, Constants.PokemonRomPath, Pokemon.DataLength) { }

        public IPokemon Retrieve(PokemonId id)
        {
            return new Pokemon(RetrieveData((int)id));
        }

        public void Save(PokemonId id, IPokemon model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
