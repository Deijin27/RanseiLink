using Core;
using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class PokemonSelectorViewModel : SelectorViewModelBase<PokemonId, IPokemon, PokemonViewModel>
    {
        public PokemonSelectorViewModel(IDialogService dialogService, PokemonId initialSelected, IModelDataService<PokemonId, IPokemon> dataService)
            : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray()) { }
    }
}
