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
            get => GetPaddedUtf8String(0, 15);
            set => SetPaddedUtf8String(0, 15, value);
        }

        public IEventSpeaker Clone()
        {
            return new EventSpeaker((byte[])Data.Clone());
        }
    }
}
