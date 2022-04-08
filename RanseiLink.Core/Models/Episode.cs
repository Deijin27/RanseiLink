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
        get => GetUInt32(0, 0, 9);
        set => SetUInt32(0, 0, 9, value);
    }

    public ScenarioId Scenario
    {
        get => (ScenarioId)GetUInt32(0, 9, 4);
        set => SetUInt32(0, 9, 4, (uint)value);
    }

    public EpisodeId UnlockCondition
    {
        get => (EpisodeId)GetUInt32(1, 0, 6);
        set => SetUInt32(1, 0, 6, (uint)value);
    }

    public uint Difficulty
    {
        get => GetUInt32(1, 6, 3);
        set => SetUInt32(1, 6, 3, value);
    }
}
