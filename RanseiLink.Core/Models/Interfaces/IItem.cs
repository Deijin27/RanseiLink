using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IItem : IDataWrapper, ICloneable<IItem>
{
    string Name { get; set; }
    uint ShopPriceMultiplier { get; set; }
    uint QuantityForEffect { get; set; }
    ItemId CraftingIngredient1 { get; set; }
    uint CraftingIngredient1Amount { get; set; }
    ItemId CraftingIngredient2 { get; set; }
    uint CraftingIngredient2Amount { get; set; }
    ItemId UnknownItem { get; set; }
    uint BuildingLevel { get; set; }
    ItemCategoryId Category { get; set; }
    ItemEffectId Effect { get; set; }
    uint EffectDuration { get; set; }

    bool GetPurchasable(KingdomId kingdom);
    void SetPurchasable(KingdomId kingdom, bool value);
}
