using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;

namespace RanseiWpf.ViewModels
{
    public class MoveSelectorViewModel : SelectorViewModelBase<MoveId, IMove, MoveViewModel>
    {
        public MoveSelectorViewModel(IDialogService dialogService, MoveId initialSelected, IModelDataService<MoveId, IMove> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
