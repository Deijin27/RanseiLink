using RanseiLink.Core.Services.ModelServices;
using System.Numerics;

namespace RanseiLink.GuiCore.ViewModels;

public interface ISelectorViewModelFactory
{
    SelectorViewModel Create(IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId, bool scrollEnabled = true);
    SelectorViewModel Create(IModelService service, object nestedViewModel, Action<int> onSelectedChanged, bool scrollEnabled = true);
}

public class SelectorViewModelFactory(IClipboardService clipboardService, IAsyncDialogService dialogService) : ISelectorViewModelFactory
{
    public SelectorViewModel Create(IModelService service, object nestedViewModel, Action<int> onSelectedChanged, bool scrollEnabled = true)
    {
        if (scrollEnabled)
        {
            return new SelectorViewModel(
                dialogService,
                clipboardService,
                service,
                service.GetComboBoxItemsExceptDefault(),
                nestedViewModel,
                onSelectedChanged,
                service.ValidateId
                );
        }
        else
        {
            return new SelectorViewModelWithoutScroll(
                dialogService,
                clipboardService,
                service,
                service.GetComboBoxItemsExceptDefault(),
                nestedViewModel,
                onSelectedChanged,
                service.ValidateId
                );
        }
    }

    public SelectorViewModel Create(IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId, bool scrollEnabled = true)
    {
        if (scrollEnabled)
        {
            return new SelectorViewModel(
                dialogService,
                clipboardService,
                service,
                displayItems,
                nestedViewModel,
                onSelectedChanged,
                validateId
                );
        }
        else
        {
            return new SelectorViewModelWithoutScroll(
                dialogService,
                clipboardService,
                service,
                displayItems,
                nestedViewModel,
                onSelectedChanged,
                validateId
                );
        }
    }

}
