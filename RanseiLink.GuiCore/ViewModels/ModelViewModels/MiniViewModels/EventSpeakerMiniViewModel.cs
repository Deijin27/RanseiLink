#nullable enable
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class EventSpeakerMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly EventSpeaker _model;
    private readonly int _id;

    public EventSpeakerMiniViewModel(
        ICachedSpriteProvider spriteProvider,
        EventSpeaker model, 
        int id, 
        ICommand selectCommand)
    {
        _spriteProvider = spriteProvider;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public int Id => _id;

    public string Name
    {
        get => _model.Name;
    }

    public object? Image => _spriteProvider.GetSprite(SpriteType.StlBushouS, _model.Sprite);

    public ICommand SelectCommand { get; }

    public bool MatchSearchTerm(string searchTerm)
    {
        if (Name.ContainsIgnoreCaseAndAccents(searchTerm))
        {
            return true;
        }

        return false;
    }

    public void NotifyPropertyChanged(string? name)
    {
        switch (name)
        {
            case nameof(EventSpeakerViewModel.Name):
                RaisePropertyChanged(nameof(Name));
                break;
            case nameof(EventSpeakerViewModel.Sprite):
                RaisePropertyChanged(nameof(Image));
                break;
        }
    }
}