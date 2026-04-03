using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class ScenarioBuilding : BaseDataWindow
{
    public const int DataLength = 0xEE;
    /// <summary>
    /// Slots per kingdom
    /// </summary>
    public const int SlotCount = 7;
    private const int _bytesPerSlot = 2;
    private ScenarioBuilding(byte[] data) : base(data, DataLength, false)
    {
    }

    public ScenarioBuilding() : this(new byte[DataLength]) { }

    private int GetBaseOffset(int kingdom, int slot) => kingdom * SlotCount * _bytesPerSlot + slot * _bytesPerSlot;

    public int GetInitialExp(int kingdom, int slot)
    {
        return GetByte(GetBaseOffset(kingdom, slot));
    }

    public bool GetIsUnlocked(int kingdom, int slot)
    {
        return GetByte(GetBaseOffset(kingdom, slot) + 1) != 0;
    }

    public void SetInitialExp(int kingdom, int slot, int value)
    {
        SetByte(GetBaseOffset(kingdom, slot), (byte)value);
    }

    public void SetIsUnlocked(int kingdom, int slot, bool value)
    {
        SetByte(GetBaseOffset(kingdom, slot) + 1, (byte)(value ? 1 : 0));
    }

    public int GetInitialExp(KingdomId kingdom, int slot) => GetInitialExp((int)kingdom, slot);
    public bool GetIsUnlocked(KingdomId kingdom, int slot) => GetIsUnlocked((int)kingdom, slot);
    public void SetInitialExp(KingdomId kingdom, int slot, int value) => SetInitialExp((int)kingdom, slot, value);
    public void SetIsUnlocked(KingdomId kingdom, int slot, bool value) => SetIsUnlocked((int)kingdom, slot, value);
}
