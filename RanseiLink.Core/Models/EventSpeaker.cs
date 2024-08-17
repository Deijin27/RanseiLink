using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public class EventSpeaker : BaseDataWindow
{
    public static int DataLength(ConquestGameCode culture) => culture == ConquestGameCode.VPYJ ? 0xC : 0x12;

    private readonly int _cultureNameLength;
    public EventSpeaker(byte[] data, ConquestGameCode culture) : base(data, DataLength(culture)) 
    {
        _cultureNameLength = culture == ConquestGameCode.VPYJ ? 0xA : 0x10;
    }
    public EventSpeaker(ConquestGameCode culture) : this(new byte[DataLength(culture)], culture) { }

    public string Name
    {
        get => GetPaddedUtf8String(0, _cultureNameLength);
        set => SetPaddedUtf8String(0, _cultureNameLength, value);
    }

    public int Sprite
    {
        get => GetByte(_cultureNameLength + 1);
        set => SetByte(_cultureNameLength + 1, (byte)value);
    }

}
