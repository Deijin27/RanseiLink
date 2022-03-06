using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate ItemSelectorViewModel ItemSelectorViewModelFactory(IEditorContext context);

public class ItemSelectorViewModel : SelectorViewModelBase<ItemId, IItem, ItemViewModel>
{
    private readonly ItemViewModelFactory _factory;
    private readonly IEditorContext _context;
    public ItemSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Item, EnumUtil.GetValuesExceptDefaults<ItemId>().ToArray()) 
    {
        _context = context;
        _factory = container.Resolve<ItemViewModelFactory>();
        Selected = ItemId.Potion;
    }

    protected override ItemViewModel NewViewModel(IItem model) => _factory(Selected, model, _context);
}
