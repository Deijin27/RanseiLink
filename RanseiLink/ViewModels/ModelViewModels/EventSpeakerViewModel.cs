#nullable enable
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class EventSpeakerViewModel : ViewModelBase
{
    private EventSpeaker _model;
    private readonly IOverrideDataProvider _spriteProvider;

    public EventSpeakerViewModel(IOverrideDataProvider overrideSpriteProvider)
    {
        _spriteProvider = overrideSpriteProvider;
        _model = new EventSpeaker();
    }

    public void SetModel(EventSpeaker model)
    {
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public int Sprite
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

    public string SpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlBushouLL, Sprite).File;

}
