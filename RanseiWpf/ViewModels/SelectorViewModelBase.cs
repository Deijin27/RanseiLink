using Core;
using Core.Models;
using Core.Services;
using System;
using System.Collections.Generic;

namespace RanseiWpf.ViewModels
{
    public abstract class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase where TViewModel : IViewModelForModel<TModel>, new() where TModel : IDataWrapper
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
}
