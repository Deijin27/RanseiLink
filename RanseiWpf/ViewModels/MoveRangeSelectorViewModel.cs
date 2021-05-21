using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class MoveRangeSelectorViewModel : SelectorViewModelBase<MoveRangeId, IMoveRange, MoveRangeViewModel>
    {
        public MoveRangeSelectorViewModel(MoveRangeId initialSelected, IModelDataService<MoveRangeId, IMoveRange> dataService) : base(initialSelected, dataService) { }
    }
}
