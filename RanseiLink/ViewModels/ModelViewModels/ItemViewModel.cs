using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate ItemViewModel ItemViewModelFactory(ItemId id, IItem model, IEditorContext context);

public class ItemViewModel : ViewModelBase
{
    private readonly IItem _model;
    private readonly ICachedMsgBlockService _msgService;
    public ItemViewModel(ItemId id, IItem model, IEditorContext context)
    {
        Id = id;
        _msgService = context.CachedMsgBlockService;
        _model = model;
    }

    public ItemId Id { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public uint ShopPriceMultiplier // max = 511
    {
        get => _model.ShopPriceMultiplier;
        set => RaiseAndSetIfChanged(_model.ShopPriceMultiplier, value, v => _model.ShopPriceMultiplier = v);
    }
    public string Description
    {
        get => _msgService.GetItemDescription(Id);
        set => _msgService.SetItemDescription(Id, value);
    }
}
