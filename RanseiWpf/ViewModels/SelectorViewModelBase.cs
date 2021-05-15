using Core;
using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using Core.Services;
using System;
using System.Collections.Generic;

namespace RanseiWpf.ViewModels
{
    public class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase where TViewModel : IViewModelForModel<TModel>, new() where TModel : IDataWrapper
    {
        public SelectorViewModelBase(TId initialSelected, IModelDataService<TId, TModel> dataService)
        {
            DataService = dataService;
            Selected = initialSelected;
        }

        private readonly IModelDataService<TId, TModel> DataService;

        TViewModel _nestedViewModel;
        public TViewModel NestedViewModel
        {
            get => _nestedViewModel;
            set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
        }

        public IEnumerable<TId> Items { get; } = EnumUtil.GetValues<TId>();

        TId _selected;
        public TId Selected
        {
            get => _selected;
            set
            {
                if (!Cache.TryGetValue(value, out TModel model))
                {
                    model = DataService.Retrieve(value);
                    Cache[value] = model;
                }
                NestedViewModel = new TViewModel() { Model = model };
                _selected = value;
            }
        }

        public Dictionary<TId, TModel> Cache = new Dictionary<TId, TModel>();

        public void SaveAndClearCache()
        {
            foreach (var (key, value) in Cache)
            {
                DataService.Save(key, value);
            }
        }

        public void ClearUnsavedChanges()
        {
            Cache.Clear();
            Selected = Selected;
        }
    }

    public class PokemonSelectorViewModel : SelectorViewModelBase<PokemonId, IPokemon, PokemonViewModel>
    {
        public PokemonSelectorViewModel(PokemonId initialSelected, IModelDataService<PokemonId, IPokemon> dataService) : base(initialSelected, dataService) { }
    }

    public class WazaSelectorViewModel : SelectorViewModelBase<MoveId, Move, WazaViewModel>
    {
        public WazaSelectorViewModel(MoveId initialSelected, IModelDataService<MoveId, Move> dataService) : base(initialSelected, dataService) { }
    }

    public class AbilitySelectorViewModel : SelectorViewModelBase<AbilityId, Ability, AbilityViewModel>
    {
        public AbilitySelectorViewModel(AbilityId initialSelected, IModelDataService<AbilityId, Ability> dataService) : base(initialSelected, dataService) { }
    }
}
