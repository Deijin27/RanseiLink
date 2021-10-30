using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;

namespace RanseiWpf.ViewModels
{
    public class MoveRangeSelectorViewModel : SelectorViewModelBase<MoveRangeId, IMoveRange, MoveRangeViewModel>
    {
        public MoveRangeSelectorViewModel(IDialogService dialogService, MoveRangeId initialSelected, IModelDataService<MoveRangeId, IMoveRange> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
