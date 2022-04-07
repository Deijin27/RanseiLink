using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

/// <summary>
/// Tokusei
/// </summary>
public class Episode : BaseDataWindow
{
    public const int DataLength = 0x8;
    public Episode(byte[] data) : base(data, DataLength) { }
    public Episode() : this(new byte[DataLength]) { }

    public uint Order
    {
        get => GetUInt32(0, 9, 0);
        set => SetUInt32(0, 9, 0, value);
    }

    public ScenarioId Scenario
    {
        get => (ScenarioId)GetUInt32(0, 4, 9);
        set => SetUInt32(0, 4, 9, (uint)value);
    }

    public EpisodeId UnlockCondition
    {
        get => (EpisodeId)GetUInt32(1, 6, 0);
        set => SetUInt32(1, 6, 0, (uint)value);
    }

    public uint Difficulty
    {
        get => GetUInt32(1, 3, 6);
        set => SetUInt32(1, 3, 6, value);
    }
}
