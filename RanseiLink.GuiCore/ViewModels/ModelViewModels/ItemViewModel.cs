#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class ItemViewModel : ViewModelBase
{
    private readonly ICachedMsgBlockService _msgService;
    public ItemViewModel(ICachedMsgBlockService msgService, IJumpService jumpService, IIdToNameService idToNameService)
    {
        _msgService = msgService;
        _model = new Item();

        JumpToItemCommand = new RelayCommand<int>(id => jumpService.JumpTo(ItemSelectorEditorModule.Id, id));

        ItemItems = idToNameService.GetComboBoxItemsPlusDefault<IItemService>();
    }

    public void SetModel(ItemId id, Item model)
    {
        Id = (int)id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public List<SelectorComboBoxItem> ItemItems { get; }

    public ICommand JumpToItemCommand { get; }

    public int Id { get; private set; }

    public string Description
    {
        get => _msgService.GetMsgOfType(MsgShortcut.ItemDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.ItemDescription, Id, value);
    }
    public string Description2
    {
        get => _msgService.GetMsgOfType(MsgShortcut.ItemDescription2, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.ItemDescription2, Id, value);
    }

    #region Purchasable

    public bool Purchasable_Aurora
    {
        get => _model.GetPurchasable(KingdomId.Aurora);
        set => _model.SetPurchasable(KingdomId.Aurora, value);
    }

    public bool Purchasable_Ignis
    {
        get => _model.GetPurchasable(KingdomId.Ignis);
        set => _model.SetPurchasable(KingdomId.Ignis, value);
    }

    public bool Purchasable_Fontaine
    {
        get => _model.GetPurchasable(KingdomId.Fontaine);
        set => _model.SetPurchasable(KingdomId.Fontaine, value);
    }

    public bool Purchasable_Violight
    {
        get => _model.GetPurchasable(KingdomId.Violight);
        set => _model.SetPurchasable(KingdomId.Violight, value);
    }

    public bool Purchasable_Greenleaf
    {
        get => _model.GetPurchasable(KingdomId.Greenleaf);
        set => _model.SetPurchasable(KingdomId.Greenleaf, value);
    }

    public bool Purchasable_Nixtorm
    {
        get => _model.GetPurchasable(KingdomId.Nixtorm);
        set => _model.SetPurchasable(KingdomId.Nixtorm, value);
    }

    public bool Purchasable_Pugilis
    {
        get => _model.GetPurchasable(KingdomId.Pugilis);
        set => _model.SetPurchasable(KingdomId.Pugilis, value);
    }

    public bool Purchasable_Viperia
    {
        get => _model.GetPurchasable(KingdomId.Viperia);
        set => _model.SetPurchasable(KingdomId.Viperia, value);
    }

    public bool Purchasable_Terrera
    {
        get => _model.GetPurchasable(KingdomId.Terrera);
        set => _model.SetPurchasable(KingdomId.Terrera, value);
    }

    public bool Purchasable_Avia
    {
        get => _model.GetPurchasable(KingdomId.Avia);
        set => _model.SetPurchasable(KingdomId.Avia, value);
    }

    public bool Purchasable_Illusio
    {
        get => _model.GetPurchasable(KingdomId.Illusio);
        set => _model.SetPurchasable(KingdomId.Illusio, value);
    }

    public bool Purchasable_Chrysalia
    {
        get => _model.GetPurchasable(KingdomId.Chrysalia);
        set => _model.SetPurchasable(KingdomId.Chrysalia, value);
    }

    public bool Purchasable_Cragspur
    {
        get => _model.GetPurchasable(KingdomId.Cragspur);
        set => _model.SetPurchasable(KingdomId.Cragspur, value);
    }

    public bool Purchasable_Spectra
    {
        get => _model.GetPurchasable(KingdomId.Spectra);
        set => _model.SetPurchasable(KingdomId.Spectra, value);
    }

    public bool Purchasable_Dragnor
    {
        get => _model.GetPurchasable(KingdomId.Dragnor);
        set => _model.SetPurchasable(KingdomId.Dragnor, value);
    }

    public bool Purchasable_Yakasha
    {
        get => _model.GetPurchasable(KingdomId.Yakasha);
        set => _model.SetPurchasable(KingdomId.Yakasha, value);
    }

    public bool Purchasable_Valora
    {
        get => _model.GetPurchasable(KingdomId.Valora);
        set => _model.SetPurchasable(KingdomId.Valora, value);
    }

    #endregion
}
