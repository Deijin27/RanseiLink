using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using System.Linq;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate PokemonSelectorViewModel PokemonSelectorViewModelFactory(IEditorContext context);

public class PokemonSelectorViewModel : SelectorViewModelBase<PokemonId, IPokemon, PokemonViewModel>
{
    private readonly PokemonViewModelFactory _factory;
    private readonly IEditorContext _editorContext;
    public PokemonSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Pokemon, EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray()) 
    { 
        _editorContext = context;
        _factory = container.Resolve<PokemonViewModelFactory>();
        Selected = PokemonId.Eevee;
    }

    protected override PokemonViewModel NewViewModel(IPokemon model) => _factory(model, _editorContext);
}
