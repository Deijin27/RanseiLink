using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;

namespace RanseiWpf.ViewModels
{
    public class SaihaiSelectorViewModel : SelectorViewModelBase<SaihaiId, ISaihai, SaihaiViewModel>
    {
        public SaihaiSelectorViewModel(SaihaiId initialSelected, IModelDataService<SaihaiId, ISaihai> dataService) : base(initialSelected, dataService) { }
    }
}
