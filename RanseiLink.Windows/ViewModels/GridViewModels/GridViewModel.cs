using System.Collections.ObjectModel;

namespace RanseiLink.Windows.ViewModels;

public interface IGridViewModel<TItemViewModel>
{
    ObservableCollection<TItemViewModel> Items { get; }
    int FrozenColumnCount { get; }
}

//public class GridViewModel : IGridViewModel<object>
//{
//    private readonly Func<int, object, object> _viewModelFactory;
//    private readonly IModelService _service;
//    private readonly IDialogService _dialogService;
//    public GridViewModel(IServiceContainer container, IModelService service, Func<int, object, object> viewModelFactory, int frozenColumnCount = 2)
//    {
//        FrozenColumnCount = frozenColumnCount;
//        _dialogService = container.Resolve<IDialogService>();
//        _viewModelFactory = viewModelFactory;
//        _service = service;
//        Refresh();
//    }

//    public ObservableCollection<object> Items { get; } = new();
//    public int FrozenColumnCount { get; }

//    public void Refresh()
//    {
//        try
//        {
//            Items.Clear();
//            foreach (int id in _service.ValidIds())
//            {
//                var model = _service.RetrieveObject(id);
//                Items.Add(_viewModelFactory(id, model));
//            }
//        }
//        catch (Exception e)
//        {
//            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
//                $"Error Refreshing Items in {GetType()}",
//                e.ToString(),
//                MessageBoxType.Error
//                ));
//        }
//    }

//    public void Save()
//    {
//        try
//        {
//            _service.Save();
//        }
//        catch (Exception e)
//        {
//            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
//                $"Error Saving Items in {GetType()}",
//                e.ToString(),
//                MessageBoxType.Error
//                ));
//        }
        
//    }
//}