using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public interface ISelectorViewModelFactory
{
    SelectorViewModel Create(IModelService? service, List<SelectorComboBoxItem> displayItems, object nestedViewModel, Action<int> onSelectedChanged, Func<int, bool> validateId, bool scrollEnabled = true);
    SelectorViewModel Create(IModelService service, object nestedViewModel, Action<int> onSelectedChanged, bool scrollEnabled = true);
    WorkspaceViewModel CreateWorkspace(IBigViewModel bigViewModel, IModelService modelService, Func<ICommand, List<IMiniViewModel>> allMiniVms);
}

public class SelectorViewModelFactory(IClipboardService clipboardService, IAsyncDialogService asyncDialogService) : ISelectorViewModelFactory
{
    public WorkspaceViewModel CreateWorkspace(IBigViewModel bigViewModel, IModelService modelService, Func<ICommand, List<IMiniViewModel>> allMiniVms)
    {
        return new WorkspaceViewModel(
            NewCopyPasteVm(),
            bigViewModel,
            modelService,
            allMiniVms
            );
    }

    private CopyPasteViewModel NewCopyPasteVm()
    {
        return new CopyPasteViewModel(clipboardService, asyncDialogService);
    }

    public SelectorViewModel Create(IModelService service, object nestedViewModel, Action<int> onSelectedChanged, bool scrollEnabled = true)
    {
        if (scrollEnabled)
        {
            return new SelectorViewModel(
                NewCopyPasteVm(),
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
                NewCopyPasteVm(),
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
                NewCopyPasteVm(),
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
                NewCopyPasteVm(),
                service,
                displayItems,
                nestedViewModel,
                onSelectedChanged,
                validateId
                );
        }
    }

}
