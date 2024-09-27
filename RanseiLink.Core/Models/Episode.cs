using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public partial class Episode
{
    public bool IsStartKingdom(KingdomId kingdomId)
    {
        return IsStartKingdom((int)kingdomId);
    }

    public void SetIsStartKingdom(KingdomId kingdomId, bool isStartKingdom)
    {
        SetIsStartKingdom((int)kingdomId, isStartKingdom);
    }

    public bool IsUnlockedKingdom(KingdomId kingdomId)
    {
        return IsUnlockedKingdom((int)kingdomId);
    }

    public void SetIsUnlockedKingdom(KingdomId kingdomId, bool isLockedKingdom)
    {
        SetIsUnlockedKingdom((int)kingdomId, isLockedKingdom);
    }

    public bool IsStartKingdom(int kingdomId)
    {
        return GetInt(0, 13 + kingdomId, 1) == 1;
    }

    public void SetIsStartKingdom(int kingdomId, bool isStartKingdom)
    {
        SetInt(0, 13 + kingdomId, 1, isStartKingdom ? 1 : 0);
    }

    public bool IsUnlockedKingdom(int kingdomId)
    {
        return GetInt(1, 9 + kingdomId, 1) == 1;
    }

    public void SetIsUnlockedKingdom(int kingdomId, bool isLockedKingdom)
    {
        SetInt(1, 9 + kingdomId, 1, isLockedKingdom ? 1 : 0);
    }
}