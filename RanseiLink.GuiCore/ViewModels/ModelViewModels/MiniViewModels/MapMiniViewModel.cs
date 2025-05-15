#nullable enable
using RanseiLink.Core.Maps;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class MapMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly INicknameService _nicknameService;
    private PSLM _model;
    private readonly MapId _id;
    private readonly string _stringId;
    private readonly int _intId;

    public MapMiniViewModel(ICachedSpriteProvider cachedSpriteProvider, INicknameService nicknameService, PSLM model, MapId id, ICommand selectCommand)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _nicknameService = nicknameService;
        _model = model;
        _id = id;
        _intId = (int)id;
        _stringId = id.ToString()[3..];
        SelectCommand = selectCommand;
        cachedSpriteProvider.OnMapMiniPreviewImageInvalidated += CachedSpriteProvider_OnMapMiniPreviewImageInvalidated;
    }

    private void CachedSpriteProvider_OnMapMiniPreviewImageInvalidated(MapId obj)
    {
        RaisePropertyChanged(nameof(Image));
    }

    public void SetModel(PSLM model)
    {
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public string StringId => _stringId;
    public int Id => _intId;

    public string Name
    {
        get => _nicknameService.GetNickname(nameof(MapId), _intId);
    }

    public object? Image => _cachedSpriteProvider.GetMapMiniPreviewImage(_id);

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
            case nameof(MapViewModel.Nickname):
                RaisePropertyChanged(nameof(Name));
                break;
        }
    }
}
