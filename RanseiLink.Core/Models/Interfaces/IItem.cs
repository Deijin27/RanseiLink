using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IItem : IDataWrapper, ICloneable<IItem>
{
    string Name { get; set; }
    uint ShopPriceMultiplier { get; set; }

    bool GetPurchasable(KingdomId kingdom);
    void SetPurchasable(KingdomId kingdom, bool value);
}
