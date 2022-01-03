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
