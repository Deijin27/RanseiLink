using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RanseiLink.ViewModels;

public interface IGridViewModel<TItemViewModel>
{
    ObservableCollection<TItemViewModel> Items { get; }
    int FrozenColumnCount { get; }
}

public abstract class GridViewModelBase<TId, TModel, TGridItemViewModel, TDataService> : IGridViewModel<TGridItemViewModel>, ISaveableRefreshable
    where TDataService : IModelDataService<TId, TModel>, IDisposable
{
    private readonly Func<TDataService> _serviceFactory;
    private readonly Func<TId, TModel, TGridItemViewModel> _factory;
    private readonly List<TModel> _models = new();
    private readonly TId[] _ids;
    private readonly IDialogService _dialogService;

    protected GridViewModelBase(IServiceContainer container, Func<TDataService> serviceFactory, TId[] ids, Func<TId, TModel, TGridItemViewModel> factory)
    {
        _factory = factory;
        _ids = ids;
        _serviceFactory = serviceFactory;
        _dialogService = container.Resolve<IDialogService>();
        Refresh();
    }

    public ObservableCollection<TGridItemViewModel> Items { get; } = new();
    public virtual int FrozenColumnCount => 1;

    public void Refresh()
    {
        try
        {
            _models.Clear();
            Items.Clear();
            using var service = _serviceFactory();
            foreach (TId id in _ids)
            {
                var model = service.Retrieve(id);
                _models.Add(model);
                Items.Add(_factory(id, model));
            }
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                $"Error Refreshing Items in {GetType()}",
                e.ToString(),
                MessageBoxType.Error
                ));
        }
    }

    public void Save()
    {
        try
        {
            using var service = _serviceFactory();
            foreach (var (id, model) in _ids.Zip(_models))
            {
                service.Save(id, model);
            }
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                $"Error Saving Items in {GetType()}",
                e.ToString(),
                MessageBoxType.Error
                ));
        }
        
    }
}