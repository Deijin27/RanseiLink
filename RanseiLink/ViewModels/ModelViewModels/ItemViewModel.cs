using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.ViewModels;

public delegate ItemViewModel ItemViewModelFactory(IItem model);

public class ItemViewModel : ViewModelBase
{
    private readonly IItem _model;

    public ItemViewModel(IItem model)
    {
        _model = model;
    }

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
}
