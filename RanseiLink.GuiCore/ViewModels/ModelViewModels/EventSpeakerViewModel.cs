#nullable enable
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public partial class EventSpeakerViewModel : ViewModelBase
{
    private readonly IOverrideDataProvider _spriteProvider;

    public EventSpeakerViewModel(IOverrideDataProvider overrideSpriteProvider)
    {
        _spriteProvider = overrideSpriteProvider;

        this.PropertyChanged += EventSpeakerViewModel_PropertyChanged;
    }

    private void EventSpeakerViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Sprite):
                RaisePropertyChanged(nameof(SpritePath));
                break;
        }
    }

    public void SetModel(EventSpeaker model)
    {
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public string SpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlBushouLL, Sprite).File;
}
