using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IEventSpeaker : IDataWrapper, ICloneable<IEventSpeaker>
{
    string Name { get; set; }
    WarriorSpriteId Sprite { get; set; }
}
