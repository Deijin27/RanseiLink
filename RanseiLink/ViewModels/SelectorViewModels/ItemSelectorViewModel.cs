﻿using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate ItemSelectorViewModel ItemSelectorViewModelFactory(IEditorContext context);

public class ItemSelectorViewModel : SelectorViewModelBase<ItemId, IItem, ItemViewModel>
{
    private readonly ItemViewModelFactory _factory;

    public ItemSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Item) 
    {
        _factory = container.Resolve<ItemViewModelFactory>();
        Selected = ItemId.Potion;
    }

    protected override ItemViewModel NewViewModel(IItem model) => _factory(model);
}