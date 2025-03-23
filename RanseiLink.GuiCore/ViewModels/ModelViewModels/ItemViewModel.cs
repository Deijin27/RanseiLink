#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class ItemViewModel : ViewModelBase, IBigViewModel
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IIdToNameService _idToNameService;

    public ItemViewModel(ICachedMsgBlockService msgService, IJumpService jumpService, IIdToNameService idToNameService)
    {
        _msgService = msgService;
        _idToNameService = idToNameService;
        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemWorkspaceModule.Id, id));

        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();
    }

    public void SetModel(ItemId id, Item model)
    {
        _id = id;
        _model = model;

        PurchasableItems.Clear();
        foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            string kingdomName = _idToNameService.IdToName<IKingdomService>((int)kingdom);
            PurchasableItems.Add(new CheckBoxViewModel(kingdomName,
                () => _model.GetPurchasable(kingdom),
                v => _model.SetPurchasable(kingdom, v)
                ));
        }

        RaiseAllPropertiesChanged();
    }

    public void SetModel(int id, object model)
    {
        SetModel((ItemId)id, (Item)model);
    }

    public ICommand JumpToItemCommand { get; }
}
