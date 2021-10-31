using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels
{
    public class AppearItem : ViewModelBase
    {
        private readonly IScenarioAppearPokemon _model;
        public AppearItem(IScenarioAppearPokemon model, PokemonId id)
        {
            _model = model;
            Pokemon = id;
        }

        public bool CanAppear
        {
            get => _model.GetCanAppear(Pokemon);
            set => RaiseAndSetIfChanged(CanAppear, value, v => _model.SetCanAppear(Pokemon, v));
        }

        public PokemonId Pokemon { get; set; }
    }

    public class ScenarioAppearPokemonViewModel : ViewModelBase, IViewModelForModel<IScenarioAppearPokemon>
    {
        private IScenarioAppearPokemon _model;
        public IScenarioAppearPokemon Model 
        {
            get => _model;
            set
            {
                _model = value;
                AppearItems = EnumUtil.GetValuesExceptDefaults<PokemonId>().Select(i => new AppearItem(value, i)).ToList();
            }
        }


        private List<AppearItem> _appearItems;
        public List<AppearItem> AppearItems
        {
            get => _appearItems;
            set => RaiseAndSetIfChanged(ref _appearItems, value);
        }
    }
}
