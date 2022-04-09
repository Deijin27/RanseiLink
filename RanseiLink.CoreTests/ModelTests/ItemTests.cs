using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using Xunit;

namespace RanseiLink.CoreTests.ModelTests;

public class ItemTests
{
    [Fact]
    public void AccessorsReturnCorrectValues()
    {
        Item a = new Item(new byte[]
        {
                0x50, 0x6F, 0x74, 0x69,
                0x6F, 0x6E, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x26, 0x40, 0x81,
                0x86, 0x00, 0x86, 0x00,
                0x86, 0x06, 0x50, 0xE0,
                0xEF, 0xDB, 0x00, 0x00,
        });

        Assert.Equal("Potion", a.Name);
        Assert.Equal(3, a.ShopPriceMultiplier);
        Assert.Equal(20, a.QuantityForEffect);

        Assert.True(a.GetPurchasable(KingdomId.Fontaine));
        Assert.False(a.GetPurchasable(KingdomId.Greenleaf));

    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Item a = new Item
        {
            Name = "Potion",
            ShopPriceMultiplier = 12,
            QuantityForEffect = 8
        };

        a.SetPurchasable(KingdomId.Chrysalia, true);
        a.SetPurchasable(KingdomId.Dragnor, false);

        Assert.Equal("Potion", a.Name);
        Assert.Equal(12, a.ShopPriceMultiplier);
        Assert.Equal(8, a.QuantityForEffect);
        Assert.True(a.GetPurchasable(KingdomId.Chrysalia));
        Assert.False(a.GetPurchasable(KingdomId.Dragnor));
    }
}
