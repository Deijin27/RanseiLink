using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate EventSpeakerViewModel EventSpeakerViewModelFactory(IEventSpeaker model, IEditorContext context);

public class EventSpeakerViewModel : ViewModelBase
{
    private readonly IEventSpeaker _model;
    private readonly IOverrideSpriteProvider _spriteProvider;

    public EventSpeakerViewModel(IEventSpeaker model, IServiceContainer container, IEditorContext context)
    {
        _spriteProvider = context.DataService.OverrideSpriteProvider;
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

    public string SpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlBushouLL, Sprite).File;

}
