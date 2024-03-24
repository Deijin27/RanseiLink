using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public interface ISelectorViewModelFactory
{
    SelectorViewModel Create(IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId, bool scrollEnabled = true);
    SelectorViewModel Create(IModelService service, object nestedViewModel, Action<int> onSelectedChanged, bool scrollEnabled = true);
}

public class SelectorViewModelFactory(CopyPasteViewModel copyPasteVm) : ISelectorViewModelFactory
{
    public SelectorViewModel Create(IModelService service, object nestedViewModel, Action<int> onSelectedChanged, bool scrollEnabled = true)
    {
        if (scrollEnabled)
        {
            return new SelectorViewModel(
                copyPasteVm,
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
                copyPasteVm,
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
                copyPasteVm,
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
                copyPasteVm,
                service,
                displayItems,
                nestedViewModel,
                onSelectedChanged,
                validateId
                );
        }
    }

}
