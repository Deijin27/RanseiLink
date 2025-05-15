#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Util;
using RanseiLink.GuiCore.Constants;

namespace RanseiLink.GuiCore.ViewModels;

public class BattleConfigMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly IMapService _mapService;
    private readonly IPathToImageConverter _pathToImageConverter;
    private readonly IMapMiniPreviewImageGenerator _mapMiniPreviewImageGenerator;
    private readonly INicknameService _nicknameService;
    private readonly BattleConfig _model;
    private readonly int _id;

    public BattleConfigMiniViewModel(IMapService mapService, IPathToImageConverter pathToImageConverter, IMapMiniPreviewImageGenerator mapMiniPreviewImageGenerator, INicknameService nicknameService, BattleConfig model, int id, ICommand selectCommand)
    {
        _mapService = mapService;
        _pathToImageConverter = pathToImageConverter;
        _mapMiniPreviewImageGenerator = mapMiniPreviewImageGenerator;
        _nicknameService = nicknameService;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public object? Image => _pathToImageConverter.TryConvert(_mapMiniPreviewImageGenerator.Generate(_mapService.Retrieve((int)_model.MapId)));

    public int Id => _id;

    public string Name => _nicknameService.GetNickname(nameof(BattleConfigId), _id);

    public Rgb15 UpperAtmosphereColor => _model.UpperAtmosphereColor;
    public Rgb15 MiddleAtmosphereColor => _model.MiddleAtmosphereColor;
    public Rgb15 LowerAtmosphereColor => _model.LowerAtmosphereColor;
    public int NumberOfTurns => _model.NumberOfTurns;

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
            case nameof(Name):
            case nameof(UpperAtmosphereColor):
            case nameof(MiddleAtmosphereColor):
            case nameof(LowerAtmosphereColor):
            case nameof(NumberOfTurns):
                RaisePropertyChanged(name);
                break;
            case nameof(BattleConfigViewModel.MapId):
                RaisePropertyChanged(nameof(Image));
                break;

        }
    }
}
