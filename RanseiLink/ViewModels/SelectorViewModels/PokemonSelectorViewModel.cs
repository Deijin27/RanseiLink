using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate PokemonSelectorViewModel PokemonSelectorViewModelFactory(IPokemonService service);

public class PokemonSelectorViewModel : SelectorViewModelBase<PokemonId, IPokemon, PokemonViewModel>
{
    private readonly PokemonViewModelFactory _factory;
    public PokemonSelectorViewModel(IServiceContainer container, IPokemonService dataService)
        : base(container, dataService, EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray()) 
    { 
        _factory = container.Resolve<PokemonViewModelFactory>();
        Selected = PokemonId.Eevee;
    }

    protected override PokemonViewModel NewViewModel(IPokemon model) => _factory(model);
}
