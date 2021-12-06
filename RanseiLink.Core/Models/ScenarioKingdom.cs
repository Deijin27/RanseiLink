using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class ScenarioKingdom : BaseDataWindow, IScenarioKingdom
{
    public const int DataLength = 0x11;
    public ScenarioKingdom(byte[] data) : base(data, DataLength) { }
    public ScenarioKingdom() : this(new byte[DataLength]) { }

    public uint GetArmy(KingdomId kingdom)
    {
        return GetByte((int)kingdom);
    }

    public void SetArmy(KingdomId kingdom, uint armyId)
    {
        SetByte((int)kingdom, (byte)armyId);
    }

    public IScenarioKingdom Clone()
    {
        return new ScenarioKingdom((byte[])Data.Clone());
    }
}
