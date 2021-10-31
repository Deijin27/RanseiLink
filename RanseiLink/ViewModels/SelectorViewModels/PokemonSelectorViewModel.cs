using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels
{
    public class PokemonSelectorViewModel : SelectorViewModelBase<PokemonId, IPokemon, PokemonViewModel>
    {
        public PokemonSelectorViewModel(IDialogService dialogService, PokemonId initialSelected, IModelDataService<PokemonId, IPokemon> dataService)
            : base(dialogService, initialSelected, dataService, EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray()) { }
    }
}
