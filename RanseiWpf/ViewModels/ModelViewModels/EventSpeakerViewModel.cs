using Core.Enums;
using Core;
using System.Linq;
using Core.Models.Interfaces;

namespace RanseiWpf.ViewModels
{
    public class EventSpeakerViewModel : ViewModelBase, IViewModelForModel<IEventSpeaker>
    {
        public IEventSpeaker Model { get; set; }

        public string Name
        {
            get => Model.Name;
            set => RaiseAndSetIfChanged(Model.Name, value, v => Model.Name = v);
        }

        public WarriorSpriteId[] SpriteItems { get; } = EnumUtil.GetValues<WarriorSpriteId>().ToArray();

        public WarriorSpriteId Sprite
        {
            get => Model.Sprite;
            set => RaiseAndSetIfChanged(Model.Sprite, value, v => Model.Sprite = v);
        }

    }
}
