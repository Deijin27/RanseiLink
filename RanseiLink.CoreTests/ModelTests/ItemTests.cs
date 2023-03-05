using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

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

        a.Name.Should().Be("Potion");
        a.PurchaseMethod.Should().Be(0);
        a.Category.Should().Be(ItemCategoryId.Consumable);
        a.Effect.Should().Be(ItemEffectId.IncreaseMaxHp);
        a.EffectDuration.Should().Be(0);
        a.CraftingIngredient1.Should().Be(ItemId.Default);
        a.CraftingIngredient1Amount.Should().Be(0);
        a.CraftingIngredient2.Should().Be(ItemId.Default);
        a.CraftingIngredient2Amount.Should().Be(0);
        a.UnknownItem.Should().Be(ItemId.Default);
        a.ShopPriceMultiplier.Should().Be(3);
        a.QuantityForEffect.Should().Be(20);

        a.GetPurchasable(KingdomId.Fontaine).Should().BeTrue();
        a.GetPurchasable(KingdomId.Greenleaf).Should().BeFalse();
    }

    [Fact]
    public void AccessorsSetCorrectValues()
    {
        Item a = new Item
        {
            Name = "Potion",
            PurchaseMethod = PurchaseMethodId.BuildingLevel2,
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

        a.Name.Should().Be("Potion");
        a.PurchaseMethod.Should().Be(PurchaseMethodId.BuildingLevel2);
        a.Category.Should().Be(ItemCategoryId.Equipment);
        a.Effect.Should().Be(ItemEffectId.EasierToLinkWithPokemon);
        a.EffectDuration.Should().Be(5);
        a.CraftingIngredient1.Should().Be(ItemId.ColdMedicine);
        a.CraftingIngredient1Amount.Should().Be(1);
        a.CraftingIngredient2.Should().Be(ItemId.EmeraldGrace);
        a.CraftingIngredient2Amount.Should().Be(21);
        a.UnknownItem.Should().Be(ItemId.DryIce);
        a.ShopPriceMultiplier.Should().Be(12);
        a.QuantityForEffect.Should().Be(8);

        a.GetPurchasable(KingdomId.Chrysalia).Should().BeTrue();
        a.GetPurchasable(KingdomId.Dragnor).Should().BeFalse();
    }
}
