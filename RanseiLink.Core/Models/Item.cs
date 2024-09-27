using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public partial class Item : BaseDataWindow
{
    public bool GetPurchasable(KingdomId kingdom)
    {
        return GetPurchasable((int)kingdom);
    }

    public void SetPurchasable(KingdomId kingdom, bool value)
    {
        SetPurchasable((int)kingdom, value);
    }

    public bool GetPurchasable(int kingdom)
    {
        return GetInt(8, kingdom, 1) == 1;
    }

    public void SetPurchasable(int kingdom, bool value)
    {
        SetInt(8, kingdom, 1, value ? 1 : 0);
    }

}