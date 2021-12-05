using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate ItemSelectorViewModel ItemSelectorViewModelFactory(IItemService service);

public class ItemSelectorViewModel : SelectorViewModelBase<ItemId, IItem, ItemViewModel>
{
    private readonly ItemViewModelFactory _factory;

    public ItemSelectorViewModel(IServiceContainer container, IItemService dataService)
        : base(container, dataService) 
    {
        _factory = container.Resolve<ItemViewModelFactory>();
        Selected = ItemId.Potion;
    }

    protected override ItemViewModel NewViewModel(IItem model) => _factory(model);
}
