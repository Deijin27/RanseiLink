using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class MoveSelectorViewModel : SelectorViewModelBase<MoveId, IMove, MoveViewModel>
    {
        public MoveSelectorViewModel(MoveId initialSelected, IModelDataService<MoveId, IMove> dataService) : base(initialSelected, dataService) { }
    }
}
