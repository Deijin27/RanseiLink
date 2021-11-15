using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.Core.Models;

public class ScenarioKingdom : BaseDataWindow, IScenarioKingdom
{
    public const int DataLength = 0x11;
    public ScenarioKingdom(byte[] data) : base(data, DataLength) { }
    public ScenarioKingdom() : this(new byte[DataLength]) { }

    public uint GetBattlesToUnlock(KingdomId kingdom)
    {
        return GetByte((int)kingdom);
    }

    public void SetBattlesToUnlock(KingdomId kingdom, uint value)
    {
        SetByte((int)kingdom, (byte)value);
    }

    public IScenarioKingdom Clone()
    {
        return new ScenarioKingdom((byte[])Data.Clone());
    }
}
