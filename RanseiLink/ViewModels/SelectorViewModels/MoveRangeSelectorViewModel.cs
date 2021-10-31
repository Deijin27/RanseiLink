using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels
{
    public class MoveRangeSelectorViewModel : SelectorViewModelBase<MoveRangeId, IMoveRange, MoveRangeViewModel>
    {
        public MoveRangeSelectorViewModel(IDialogService dialogService, MoveRangeId initialSelected, IModelDataService<MoveRangeId, IMoveRange> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
