using Core;
using Core.Services;
using System.Collections.Generic;

namespace RanseiWpf.ViewModels
{
    public abstract class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase, ISaveableRefreshable where TViewModel : IViewModelForModel<TModel>, new()
    {
        public SelectorViewModelBase(TId initialSelected, IModelDataService<TId, TModel> dataService)
        {
            DataService = dataService;
            Selected = initialSelected;
        }

        private readonly IModelDataService<TId, TModel> DataService;

        private TViewModel _nestedViewModel;
        public TViewModel NestedViewModel
        {
            get => _nestedViewModel;
            set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
        }

        public IEnumerable<TId> Items { get; } = EnumUtil.GetValues<TId>();

        private TId _selected;
        public TId Selected
        {
            get => _selected;
            set
            {
                Save();
                _selected = value;
                Refresh();
            }
        }

        /// <summary>
        /// Reload without saving.
        /// </summary>
        public void Refresh()
        {
            TModel model = DataService.Retrieve(_selected);
            NestedViewModel = new TViewModel() { Model = model };
        }

        public virtual void Save()
        {
            if (NestedViewModel != null && _selected != null)
            {
                DataService.Save(_selected, NestedViewModel.Model);
            }
        }
    }
}
