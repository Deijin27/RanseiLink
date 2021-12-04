using RanseiLink.Core.Enums;
using RanseiLink.Core;
using System.Linq;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.ViewModels;

public delegate EventSpeakerViewModel EventSpeakerViewModelFactory(IEventSpeaker model);

public class EventSpeakerViewModel : ViewModelBase
{
    private readonly IEventSpeaker _model;

    public EventSpeakerViewModel(IEventSpeaker model)
    {
        _model = model;
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public WarriorSpriteId[] SpriteItems { get; } = EnumUtil.GetValues<WarriorSpriteId>().ToArray();

    public WarriorSpriteId Sprite
    {
        get => _model.Sprite;
        set => RaiseAndSetIfChanged(_model.Sprite, value, v => _model.Sprite = v);
    }

}
