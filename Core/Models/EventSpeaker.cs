using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class EventSpeaker : BaseDataWindow, IEventSpeaker
    {
        public const int DataLength = 0x12;
        public EventSpeaker(byte[] data) : base(data, DataLength) { }
        public EventSpeaker() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 16);
            set => SetPaddedUtf8String(0, 16, value);
        }

        public WarriorSpriteId Sprite
        {
            get => (WarriorSpriteId)GetByte(0x11);
            set => SetByte(0x11, (byte)value);
        }

        public IEventSpeaker Clone()
        {
            return new EventSpeaker((byte[])Data.Clone());
        }
    }
}
