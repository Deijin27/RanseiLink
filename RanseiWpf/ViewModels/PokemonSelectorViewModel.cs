using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class PokemonSelectorViewModel : SelectorViewModelBase<PokemonId, IPokemon, PokemonViewModel>
    {
        public PokemonSelectorViewModel(PokemonId initialSelected, IModelDataService<PokemonId, IPokemon> dataService) : base(initialSelected, dataService) { }
    }
}
