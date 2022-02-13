using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public delegate EventSpeakerViewModel EventSpeakerViewModelFactory(IEventSpeaker model);

public class EventSpeakerViewModel : ViewModelBase
{
    private readonly IEventSpeaker _model;
    private readonly ISpriteProvider _spriteProvider;

    public EventSpeakerViewModel(IEventSpeaker model, IServiceContainer container)
    {
        _spriteProvider = container.Resolve<ISpriteProvider>();
        _model = model;
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public uint Sprite
    {
        get => _model.Sprite;
        set
        {
            if (RaiseAndSetIfChanged(_model.Sprite, value, v => _model.Sprite = v))
            {
                RaisePropertyChanged(nameof(SpritePath));
            }
        }
    }

    public string SpritePath => _spriteProvider.GetSpriteFilePath(SpriteType.StlBushouLL, Sprite);

}
