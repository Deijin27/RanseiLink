using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public class MoveSelectorViewModel : SelectorViewModelBase<MoveId, IMove, MoveViewModel>
{
    public MoveSelectorViewModel(IDialogService dialogService, MoveId initialSelected, IModelDataService<MoveId, IMove> dataService)
        : base(dialogService, initialSelected, dataService) { }
}
