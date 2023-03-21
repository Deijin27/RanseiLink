#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ItemViewModel : ViewModelBase
{
    private Item _model;
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

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public PurchaseMethodId PurchaseMethod
    {
        get => _model.PurchaseMethod;
        set => RaiseAndSetIfChanged(_model.PurchaseMethod, value, v => _model.PurchaseMethod = v);
    }

    public ItemCategoryId Category
    {
        get => _model.Category;
        set => RaiseAndSetIfChanged(_model.Category, value, v => _model.Category = v);
    }

    public ItemEffectId Effect
    {
        get => _model.Effect;
        set => RaiseAndSetIfChanged(_model.Effect, value, v => _model.Effect = v);
    }

    public int EffectDuration
    {
        get => _model.EffectDuration;
        set => RaiseAndSetIfChanged(_model.EffectDuration, value, v => _model.EffectDuration = v);
    }

    public int CraftingIngredient1
    {
        get => (int)_model.CraftingIngredient1;
        set => RaiseAndSetIfChanged(_model.CraftingIngredient1, (ItemId)value, v => _model.CraftingIngredient1 = v);
    }

    public int CraftingIngredient1Amount
    {
        get => _model.CraftingIngredient1Amount;
        set => RaiseAndSetIfChanged(_model.CraftingIngredient1Amount, value, v => _model.CraftingIngredient1Amount = v);
    }

    public int CraftingIngredient2
    {
        get => (int)_model.CraftingIngredient2;
        set => RaiseAndSetIfChanged(_model.CraftingIngredient2, (ItemId)value, v => _model.CraftingIngredient2 = v);
    }

    public int CraftingIngredient2Amount
    {
        get => _model.CraftingIngredient2Amount;
        set => RaiseAndSetIfChanged(_model.CraftingIngredient2Amount, value, v => _model.CraftingIngredient2Amount = v);
    }

    public int UnknownItem
    {
        get => (int)_model.UnknownItem;
        set => RaiseAndSetIfChanged(_model.UnknownItem, (ItemId)value, v => _model.UnknownItem = v);
    }

    public int ShopPriceMultiplier // max = 511
    {
        get => _model.ShopPriceMultiplier;
        set => RaiseAndSetIfChanged(_model.ShopPriceMultiplier, value, v => _model.ShopPriceMultiplier = v);
    }

    public int QuantityForEffect
    {
        get => _model.QuantityForEffect;
        set => RaiseAndSetIfChanged(_model.QuantityForEffect, value, v => _model.QuantityForEffect = v);
    }
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
