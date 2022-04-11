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
        Assert.Equal(0, a.BuildingLevel);
        Assert.Equal(ItemCategoryId.Consumable, a.Category);
        Assert.Equal(ItemEffectId.HealsHp, a.Effect);
        Assert.Equal(0, a.EffectDuration);
        Assert.Equal(ItemId.Default, a.CraftingIngredient1);
        Assert.Equal(0, a.CraftingIngredient1Amount);
        Assert.Equal(ItemId.Default, a.CraftingIngredient2);
        Assert.Equal(0, a.CraftingIngredient2Amount);
        Assert.Equal(ItemId.Default, a.UnknownItem);
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
            BuildingLevel = 2,
            Category = ItemCategoryId.Equipment,
            Effect = ItemEffectId.EasierToLinkWithPokemon,
            EffectDuration = 5,
            CraftingIngredient1 = ItemId.ColdMedicine,
            CraftingIngredient1Amount = 1,
            CraftingIngredient2 = ItemId.EmeraldGrace,
            CraftingIngredient2Amount = 21,
            UnknownItem = ItemId.DryIce,
            ShopPriceMultiplier = 12,
            QuantityForEffect = 8
        };

        a.SetPurchasable(KingdomId.Chrysalia, true);
        a.SetPurchasable(KingdomId.Dragnor, false);

        Assert.Equal("Potion", a.Name);
        Assert.Equal(2, a.BuildingLevel);
        Assert.Equal(ItemCategoryId.Equipment, a.Category);
        Assert.Equal(ItemEffectId.EasierToLinkWithPokemon, a.Effect);
        Assert.Equal(5, a.EffectDuration);
        Assert.Equal(ItemId.ColdMedicine, a.CraftingIngredient1);
        Assert.Equal(1, a.CraftingIngredient1Amount);
        Assert.Equal(ItemId.EmeraldGrace, a.CraftingIngredient2);
        Assert.Equal(21, a.CraftingIngredient2Amount);
        Assert.Equal(ItemId.DryIce, a.UnknownItem);
        Assert.Equal(12, a.ShopPriceMultiplier);
        Assert.Equal(8, a.QuantityForEffect);

        Assert.True(a.GetPurchasable(KingdomId.Chrysalia));
        Assert.False(a.GetPurchasable(KingdomId.Dragnor));
    }
}
