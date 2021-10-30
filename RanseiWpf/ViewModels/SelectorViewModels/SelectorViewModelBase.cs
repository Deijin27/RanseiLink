using Core;
using Core.Services;
using RanseiWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public abstract class SelectorViewModelBase<TId, TModel, TViewModel> : ViewModelBase, ISaveableRefreshable where TViewModel : IViewModelForModel<TModel>, new()
    {
        private readonly IDialogService _dialogService;
        public SelectorViewModelBase(IDialogService dialogService, TId initialSelected, IModelDataService<TId, TModel> dataService)
        {
            _dialogService = dialogService;
            DataService = dataService;
            Selected = initialSelected;
            Items = EnumUtil.GetValues<TId>().ToArray();
        }

        public SelectorViewModelBase(IDialogService dialogService, TId initialSelected, IModelDataService<TId, TModel> dataService, TId[] items)
        {
            _dialogService = dialogService;
            DataService = dataService;
            Selected = initialSelected;
            Items = items;
        }

        private readonly IModelDataService<TId, TModel> DataService;

        private TViewModel _nestedViewModel;
        public TViewModel NestedViewModel
        {
            get => _nestedViewModel;
            set => RaiseAndSetIfChanged(ref _nestedViewModel, value);
        }

        public IEnumerable<TId> Items { get; }

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
            try
            {
                TModel model = DataService.Retrieve(_selected);
                NestedViewModel = new TViewModel() { Model = model };
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageBox(new MessageBoxArgs() 
                {
                    Icon = System.Windows.MessageBoxImage.Error,
                    Title = $"Error retrieving data in {GetType().Name}",
                    Message = e.Message
                });
            }
            
        }

        public virtual void Save()
        {
            if (NestedViewModel != null && _selected != null)
            {
                try
                {
                    DataService.Save(_selected, NestedViewModel.Model);
                }
                catch (Exception e)
                {
                    _dialogService.ShowMessageBox(new MessageBoxArgs()
                    {
                        Icon = System.Windows.MessageBoxImage.Error,
                        Title = $"Error saving data in {GetType().Name}",
                        Message = e.Message
                    });
                }
            }
        }
    }
}
