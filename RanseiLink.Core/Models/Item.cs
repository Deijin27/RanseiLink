using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class Item : BaseDataWindow, IItem
{
    public const int DataLength = 0x24;
    public Item(byte[] data) : base(data, DataLength)
    {
    }

    public Item() : base(new byte[DataLength], DataLength) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, 20);
        set => SetPaddedUtf8String(0, 20, value);
    }

    public uint BuildingLevel
    {
        get => GetUInt32(5, 2, 16);
        set => SetUInt32(5, 2, 16, value);
    }

    public ItemCategoryId Category
    {
        get => (ItemCategoryId)GetUInt32(5, 2, 19);
        set => SetUInt32(5, 2, 19, (uint)value);
    }

    public ItemEffectId Effect
    {
        get => (ItemEffectId)GetUInt32(5, 5, 21);
        set => SetUInt32(5, 5, 21, (uint)value);
    }

    public uint EffectDuration
    {
        get => GetUInt32(5, 3, 26);
        set => SetUInt32(5, 3, 26, value);
    }

    public ItemId CraftingIngredient1
    {
        get => (ItemId)GetUInt32(6, 9, 0);
        set => SetUInt32(6, 9, 0, (uint)value);
    }

    public uint CraftingIngredient1Amount
    {
        get => GetUInt32(6, 7, 9);
        set => SetUInt32(6, 7, 9, value);
    }

    public ItemId CraftingIngredient2
    {
        get => (ItemId)GetUInt32(6, 9, 16);
        set => SetUInt32(6, 9, 16, (uint)value);
    }

    public uint CraftingIngredient2Amount
    {
        get => GetUInt32(6, 7, 25);
        set => SetUInt32(6, 7, 25, value);
    }

    public ItemId UnknownItem
    {
        get => (ItemId)GetUInt32(7, 9, 0);
        set => SetUInt32(7, 9, 0, (uint)value);
    }

    /// <summary>
    /// Max shop price / 100
    /// </summary>
    public uint ShopPriceMultiplier
    {
        get => GetUInt32(7, 9, 9);
        set => SetUInt32(7, 9, 9, value);
    }

    public uint QuantityForEffect
    {
        get => GetUInt32(7, 9, 18);
        set => SetUInt32(7, 9, 18, value);
    }

    public bool GetPurchasable(KingdomId kingdom)
    {
        return GetUInt32(8, 1, (int)kingdom) == 1u;
    }

    public void SetPurchasable(KingdomId kingdom, bool value)
    {
        SetUInt32(8, 1, (int)kingdom, value ? 1u : 0u);
    }

    public IItem Clone()
    {
        return new Item((byte[])Data.Clone());
    }
}
