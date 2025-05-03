#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;
using RanseiLink.GuiCore.Constants;

namespace RanseiLink.GuiCore.ViewModels;

public class BattleConfigMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly INicknameService _nicknameService;
    private readonly BattleConfig _model;
    private readonly int _id;

    public BattleConfigMiniViewModel(INicknameService nicknameService, BattleConfig model, int id, ICommand selectCommand)
    {
        _nicknameService = nicknameService;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public int Id => _id;

    public string Name => _nicknameService.GetNickname(nameof(BattleConfigId), _id);

    public Rgb15 UpperAtmosphereColor => _model.UpperAtmosphereColor;
    public Rgb15 MiddleAtmosphereColor => _model.MiddleAtmosphereColor;
    public Rgb15 LowerAtmosphereColor => _model.LowerAtmosphereColor;
    public int NumberOfTurns => _model.NumberOfTurns;
    public IconId Icon
    {
        get
        {
            if (_model.VictoryCondition.HasFlag(BattleVictoryConditionFlags.HoldAllBannersFor5Turns)
                || _model.DefeatCondition.HasFlag(BattleVictoryConditionFlags.HoldAllBannersFor5Turns))
            {
                return IconId.flag_circle;
            }
            if (_model.VictoryCondition.HasFlag(BattleVictoryConditionFlags.ClaimAllBanners)
                || _model.DefeatCondition.HasFlag(BattleVictoryConditionFlags.ClaimAllBanners))
            {
                return IconId.flag;
            }
            return IconId.swords;
        }
    }

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
            case nameof(BattleConfigViewModel.VictoryCondition):
            case nameof(BattleConfigViewModel.DefeatCondition):
                RaisePropertyChanged(nameof(Icon));
                break;

        }
    }
}
