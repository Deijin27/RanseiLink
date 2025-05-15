#nullable enable
using RanseiLink.Core.Maps;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class MapMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly IMapMiniPreviewImageGenerator _mapMiniPreviewImageGenerator;
    private readonly INicknameService _nicknameService;
    private PSLM _model;
    private readonly string _stringId;
    private readonly int _id;

    public MapMiniViewModel(IPathToImageConverter pathToImageConverter, IMapMiniPreviewImageGenerator mapMiniPreviewImageGenerator, INicknameService nicknameService, PSLM model, MapId id, ICommand selectCommand)
    {
        _pathToImageConverter = pathToImageConverter;
        _mapMiniPreviewImageGenerator = mapMiniPreviewImageGenerator;
        _nicknameService = nicknameService;
        _model = model;
        _id = (int)id;
        _stringId = id.ToString()[3..];
        SelectCommand = selectCommand;
    }

    public void SetModel(PSLM model)
    {
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public string StringId => _stringId;
    public int Id => _id;

    public string Name
    {
        get => _nicknameService.GetNickname(nameof(MapId), _id);
    }

    public object? Image => _pathToImageConverter.TryConvert(_mapMiniPreviewImageGenerator.Generate(_model));

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
