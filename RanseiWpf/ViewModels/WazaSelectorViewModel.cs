using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class WazaSelectorViewModel : SelectorViewModelBase<MoveId, IMove, WazaViewModel>
    {
        public WazaSelectorViewModel(MoveId initialSelected, IModelDataService<MoveId, IMove> dataService) : base(initialSelected, dataService) { }
    }
}
