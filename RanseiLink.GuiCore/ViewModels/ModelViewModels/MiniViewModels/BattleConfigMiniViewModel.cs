#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class BattleConfigMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly INicknameService _nicknameService;
    private readonly BattleConfig _model;
    private readonly int _id;

    public BattleConfigMiniViewModel(ICachedSpriteProvider cachedSpriteProvider, INicknameService nicknameService, BattleConfig model, int id, ICommand selectCommand)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _nicknameService = nicknameService;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public object? Image => _cachedSpriteProvider.GetMapMiniPreviewImage(_model.MapId);

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
