using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class ScenarioKingdom : BaseDataWindow
{
    public const int DataLength = 0x11;
    public ScenarioKingdom(byte[] data) : base(data, DataLength) { }
    public ScenarioKingdom() : this(new byte[DataLength]) { }

    public int GetArmy(KingdomId kingdom)
    {
        return GetByte((int)kingdom);
    }

    public void SetArmy(KingdomId kingdom, int armyId)
    {
        SetByte((int)kingdom, (byte)armyId);
    }

}
